using CleanserBlazorUI.Components;
using CleanserBlazorUI.Components.Account;
using CleanserBlazorUI.Data;
using CleanserBlazorUI.Services;
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

builder.Services.AddSingleton<SessionSettingsService>();

// How soon a changed security stamp (password reset, email change by an admin) ends someone's
// existing sessions. The default is 30 minutes; a minute is far safer for confidential data.
builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(1));
builder.Services.AddSingleton<SessionActivityRegistry>();
builder.Services.AddScoped<JobTracker>();
builder.Services.ConfigureApplicationCookie(options =>
{
    // The real session limit is the admin-set idle timeout (SessionIdle); the
    // cookie's own lifetime is just a long backstop above the maximum setting.
    options.ExpireTimeSpan = TimeSpan.FromHours(9);
    options.SlidingExpiration = true;

    var identityValidate = options.Events.OnValidatePrincipal;
    options.Events.OnValidatePrincipal = async context =>
    {
        await SessionIdle.ValidateAsync(context);
        if (context.Principal is null) return;
        if (identityValidate is not null) await identityValidate(context);
    };

    var identitySigningIn = options.Events.OnSigningIn;
    options.Events.OnSigningIn = context =>
    {
        SessionIdle.StampSignIn(context);
        return identitySigningIn(context);
    };

    // The keep-alive ping and idle probe are called by script: answer 401 instead of redirecting to the login page.
    var redirectToLogin = options.Events.OnRedirectToLogin;
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/session/keepalive") || context.Request.Path.StartsWithSegments("/session/check"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        return redirectToLogin(context);
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        // ADO.NET's default 30s command timeout is too short for this app's bulk
        // reference lookups/inserts on large files -- example.appsettings.json's
        // template connection string always carried "Command Timeout=0" for this
        // reason, but that setting silently vanishes if a deployed appsettings.json
        // omits it (as happened on .2/.83), so set it here instead of relying on
        // every environment's connection string to remember it.
        sqlOptions.CommandTimeout(300)));

var xdsDataLogConnectionString = builder.Configuration.GetConnectionString("XdsDataLogDbConnection")
    ?? throw new InvalidOperationException("Connection string 'XdsDataLogDbConnection' not found.");

builder.Services.AddDbContext<XdsDataLogDbContext>(options =>
    options.UseSqlServer(xdsDataLogConnectionString, sqlOptions =>
        sqlOptions.CommandTimeout(300)));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Emails are for sign-in only: the app sends none, so the email-sending setup below is switched off.
// (The classes stay in the source; uncomment these registrations to use them again.)
// builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization();

// RETIRED -- the "PasswordChanged" policy read the old MustChangePassword true/false
// flag, which nothing ever set, and no page used the policy. First-login enforcement is
// now PasswordChangeMiddleware (the "MustChangePassword" role).
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("PasswordChanged", policy =>
//         policy.RequireAssertion(async context =>
//         {
//             var userManager = context.Resource as UserManager<ApplicationUser>;
//             if (userManager == null) return false;
//
//             var userEmail = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
//             if (userEmail == null) return false;
//
//             var user = await userManager.FindByEmailAsync(userEmail);
//             return user?.MustChangePassword ?? false;
//         }));
// });

builder.Services.AddScoped<ExcelProcessorService>();
builder.Services.AddScoped<DataManagementService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<PasswordResetRequestService>();
builder.Services.AddScoped<UserEmailService>();
builder.Services.AddSingleton<FileCleanupSettingsService>();
// Add SMTP configuration
// builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequiredLength = 8;
});

// Add to your services
// builder.Services.AddTransient<ICustomEmailSender, EmailSender>();
// builder.Services.AddTransient<IEmailSender>(sp =>
//     sp.GetRequiredService<ICustomEmailSender>());

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
// Authentication, then the new-account lock, then authorization -- in that order, so a
// brand-new account is sent to "set your password" before any page's role check can
// answer "access denied". (Spelled out because the implicit calls would put the lock
// after authorization.)
app.UseAuthentication();
app.UseMiddleware<PasswordChangeMiddleware>();
app.UseAuthorization();
app.UseAntiforgery();

// ?theme=light|dark (the sign-in page's toggle link) is remembered as the theme cookie.
app.Use(async (context, next) =>
{
    var theme = context.Request.Query["theme"].FirstOrDefault();
    if (theme is "light" or "dark")
    {
        context.Response.Cookies.Append(ThemeService.CookieName, theme, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }
    await next();
});


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapAdditionalIdentityEndpoints();

// One source of truth for generated temporary passwords (Register user's Generate button).
// Only people who can create accounts may ask; never cached.
app.MapGet("/api/password/generate", (HttpContext context) =>
{
    context.Response.Headers.CacheControl = "no-store";
    return Results.Text(TemporaryPassword.Generate());
}).RequireAuthorization(policy => policy.RequireRole("admin", "superuser"));

// Called by _session.js while the user is active; the request itself is what
// refreshes the idle timer (see SessionIdle). 204 = still active; 200 "ended" = the session has ended.
// (Answered with a normal 200 rather than 401 so a session that has simply run out does not
// show up as a red error in the browser console.)
app.MapPost("/session/keepalive", (HttpContext context) =>
        context.User.Identity?.IsAuthenticated == true ? Results.NoContent() : Results.Text("ended"))
    .DisableAntiforgery();

// _session.js asks this once its own idle window has passed. 204 = the server still
// counts the session as active (e.g. a job is running), 200 "ended" = it has ended. The cookie
// check treats this path as a pure probe, so asking never extends the session.
app.MapPost("/session/check", (HttpContext context) =>
        context.User.Identity?.IsAuthenticated == true ? Results.NoContent() : Results.Text("ended"))
    .DisableAntiforgery();

// _session.js sends an idle page here: end the session, then show the sign-in page.
app.MapGet("/session/timeout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("~/Account/Login?expired=1");
});

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
