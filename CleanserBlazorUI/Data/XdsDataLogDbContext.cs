using Microsoft.EntityFrameworkCore;

namespace CleanserBlazorUI.Data;

// Points at the separate, externally-owned XDSDataLogDB database (connection
// string "XdsDataLogDbConnection"). Read-only: this app never migrates or
// writes to these tables, only queries them.
public class XdsDataLogDbContext(DbContextOptions<XdsDataLogDbContext> options) : DbContext(options)
{
    public DbSet<ReceivedTrans> ReceivedTrans { get; set; }
    public DbSet<RingUser> RingUsers { get; set; }
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<SubscriberCategory> SubscriberCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ReceivedTrans>(e =>
        {
            e.HasNoKey();
            e.ToTable("ReceivedTrans", "Transact");
        });

        builder.Entity<RingUser>(e =>
        {
            e.HasNoKey();
            e.ToTable("Users", "Ring");
        });

        builder.Entity<Subscriber>(e =>
        {
            e.HasNoKey();
            e.ToTable("Subscribers", "Subscriber");
        });

        builder.Entity<SubscriberCategory>(e =>
        {
            e.HasNoKey();
            e.ToTable("SubscriberCategory", "Subscriber");
        });
    }
}
