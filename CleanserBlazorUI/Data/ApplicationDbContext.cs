using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CleanserBlazorUI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BusinessRef> BusinessesData { get; set; }
    public DbSet<IndividualRef> IndividualsData { get; set; }
    public DbSet<SettingsClass> Settings { get; set; }
    public DbSet<BusSettNormalizer> BusinessClassNormalizer { get; set; }
    public DbSet<SubscriberProfile> SubscriberProfiles { get; set; }
    public DbSet<UnloadableLogHeader> UnloadableLogHeaders { get; set; }
    public DbSet<UnloadableLogMessageDetail> UnloadableLogMessageDetails { get; set; }
    public DbSet<UnloadableLogCategoryDetail> UnloadableLogCategoryDetails { get; set; }
    // Moved from a separate, unmigrated "blazor-CleanserAppDB" database
    // (raw ADO.NET against [Subscriber].[Subscribers], no schema tracking)
    // into this EF-managed one. See GetShortCodeFromSubscribeIDAsync.
    public DbSet<SubscribeContext> SubscriberShortCodes { get; set; }
    public DbSet<ReferenceDataConflict> ReferenceDataConflicts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Required for UnloadableLogHeader.SubscriberProfileId to be a safe FK --
        // SaveSubscriberProfileAsync already treats SubscriberCode as unique via an
        // app-level lookup, but nothing enforced that at the DB level until now.
        builder.Entity<SubscriberProfile>()
            .HasIndex(p => p.SubscriberCode)
            .IsUnique();

        builder.Entity<UnloadableLogHeader>()
            .HasOne(h => h.SubscriberProfile)
            .WithMany()
            .HasForeignKey(h => h.SubscriberProfileId)
            .OnDelete(DeleteBehavior.Restrict); // never let a profile edit/cleanup cascade-delete log history

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
    }
}