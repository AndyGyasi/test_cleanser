using CleanserBlazorUI.Components;
using CleanserBlazorUI.Components.Account;
using CleanserBlazorUI.Data;
using CleanserBlazorUI.Models;
using CleanserBlazorUI.Services;
using ICleanserBlazorUI.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Security.Claims;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Use Serilog for logging
builder.Host.UseSerilog();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Account");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Account/ForgotPassword");
    options.Conventions.AllowAnonymousToPage("/Account/ResetPassword");
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1 * 1024 * 1024 * 1024; // 1GB
});
// The multipart form parser's own limit is 128MB by default, independent of
// Kestrel's MaxRequestBodySize above -- raise it to match, so the plain-HTTP
// spreadsheet upload endpoint (see MapPost("/api/uploads/spreadsheets")) can
// actually accept files up to that size.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1 * 1024 * 1024 * 1024; // 1GB
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Detailed circuit errors in dev only -- surfaces the real exception message
// client-side too, instead of just "circuit terminated" with no detail.
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
});
// Default SignalR circuit message size (~32KB) is far smaller than the Excel
// files InputFile streams over the circuit, silently killing the connection
// mid-upload. Raise it to match MAX_FILESIZE's realistic usage.
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 250 * 1024 * 1024; // 250MB
});
builder.Services.AddMudServices();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PasswordChanged", policy =>
        policy.RequireAssertion(async context =>
        {
            var userManager = context.Resource as UserManager<ApplicationUser>;
            if (userManager == null) return false;

            var userEmail = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null) return false;

            var user = await userManager.FindByEmailAsync(userEmail);
            return user?.MustChangePassword ?? false;
        }));
});

builder.Services.AddScoped<ExcelProcessorService>();
builder.Services.AddScoped<DataManagementService>();
builder.Services.AddSingleton<FileCleanupSettingsService>();
// Add SMTP configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// Add to your services
builder.Services.AddTransient<ICustomEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailSender>(sp =>
    sp.GetRequiredService<ICustomEmailSender>());

builder.Services.AddHostedService<ScheduledTaskService>();
builder.Services.AddHostedService<FileCleanupService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        Log.Information("Starting application initialization");
        
        // 1. Apply migrations FIRST
        Log.Information("Applying database migrations");
        dbContext.Database.Migrate();
        Log.Information("Database migrations completed successfully");

        // 2. Seed data AFTER migrations
        Log.Information("Starting data seeding");
        await SeedData.Initialize(services);
        Log.Information("Data seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Application startup failed");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapAdditionalIdentityEndpoints();

// Plain-HTTP upload used by the file dropzones instead of routing bytes
// through browserFile.OpenReadStream() over the SignalR circuit -- that path
// has a small message-size ceiling (see HubOptions.MaximumReceiveMessageSize
// above) and silently kills the circuit on real-world Excel files. This
// endpoint saves straight to the same "temp" folder/naming convention the
// Razor components already used, so the rest of the pipeline is unchanged.
app.MapPost("/api/uploads/spreadsheets", async (HttpRequest request, IWebHostEnvironment env) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest("Expected multipart/form-data.");
    }

    var form = await request.ReadFormAsync();
    var uploadDirectory = Path.Combine(env.WebRootPath, "temp");
    Directory.CreateDirectory(uploadDirectory);

    var savedPaths = new List<string>();
    foreach (var file in form.Files)
    {
        var extension = Path.GetExtension(file.FileName);
        var randomFileName = $"{Path.GetRandomFileName()}_____{file.FileName}";
        var savedPath = Path.Combine(uploadDirectory, Path.ChangeExtension(randomFileName, extension));

        await using var destination = new FileStream(savedPath, FileMode.Create);
        await file.CopyToAsync(destination);

        savedPaths.Add(savedPath);
    }

    return Results.Ok(new { paths = savedPaths });
}).RequireAuthorization();

try
{
    Log.Information("Starting web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
