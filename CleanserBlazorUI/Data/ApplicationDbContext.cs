using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CleanserBlazorUI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BusinessRef> BusinessesData { get; set; }
    public DbSet<IndividualRef> IndividualsData { get; set; }
    public DbSet<IndividualMobileRef> IndividualsMobileData { get; set; }
    public DbSet<SettingsClass> Settings { get; set; }
    public DbSet<BusSettNormalizer> BusinessClassNormalizer { get; set; }
    public DbSet<UnloadableLogHeader> UnloadableLogHeaders { get; set; }
    public DbSet<UnloadableLogMessageDetail> UnloadableLogMessageDetails { get; set; }
    public DbSet<UnloadableLogCategoryDetail> UnloadableLogCategoryDetails { get; set; }
    public DbSet<UnloadableErrorCatalogEntry> UnloadableErrorCatalogEntries { get; set; }
    public DbSet<DataLoggingGateMessages> DataLoggingGateMessages { get; set; }
    public DbSet<DataLoggingUnloggedAttempt> DataLoggingUnloggedAttempts { get; set; }
    public DbSet<DataLoggingAccessRequestReason> DataLoggingAccessRequestReasons { get; set; }
    public DbSet<DataLoggingAccessRequest> DataLoggingAccessRequests { get; set; }
    public DbSet<DataLoggingCleaningPurposeReason> DataLoggingCleaningPurposeReasons { get; set; }
    public DbSet<DataLoggingCleaningPurposeLog> DataLoggingCleaningPurposeLogs { get; set; }
    public DbSet<DataLoggingCleanOnlyLog> DataLoggingCleanOnlyLogs { get; set; }
    public DbSet<NavSection> NavSections { get; set; }
    public DbSet<NavItem> NavItems { get; set; }
    public DbSet<SessionSettings> SessionSettings { get; set; }
    public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
    // Moved from a separate, unmigrated "blazor-CleanserAppDB" database
    // (raw ADO.NET against [Subscriber].[Subscribers], no schema tracking)
    // into this EF-managed one. See GetShortCodeFromSubscribeIDAsync.
    public DbSet<ReferenceDataConflict> ReferenceDataConflicts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Retired true/false flag (see ApplicationUser). The column is NOT NULL with no
        // default, so EF must keep writing it or creating a user would fail; as a shadow
        // property it stays in the model (no migration) but no code can read it.
        builder.Entity<ApplicationUser>().Property<bool>("MustChangePassword");

        builder.Entity<UnloadableLogMessageDetail>()
            .HasOne(d => d.UnloadableLogHeader)
            .WithMany(h => h.MessageDetails)
            .HasForeignKey(d => d.UnloadableLogHeaderId)
            .OnDelete(DeleteBehavior.Cascade); // detail rows are meaningless without their header

        builder.Entity<UnloadableLogCategoryDetail>()
            .HasOne(d => d.UnloadableLogHeader)
            .WithMany(h => h.CategoryDetails)
            .HasForeignKey(d => d.UnloadableLogHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .Property(u => u.ReceivedTransUserID)
            .HasColumnType("varchar(4)")
            .HasMaxLength(4);

        // One tracked row per filename -- upserted, not one row per attempt.
        builder.Entity<DataLoggingUnloggedAttempt>()
            .HasIndex(a => a.Filename)
            .IsUnique();

        builder.Entity<NavItem>()
            .HasOne(i => i.NavSection)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.NavSectionId)
            .OnDelete(DeleteBehavior.Cascade); // deleting a section takes its items with it -- the management UI warns before that happens
    }
}