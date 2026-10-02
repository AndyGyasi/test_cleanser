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

    // Every column this app reads from the external database is mapped as text. The real tables keep some of
    // these in numeric or other column types (an ID stored as a number, for example), and reading a number
    // straight into a text property throws "Unable to cast Int32 to String" and stops the whole run. So each
    // table is read through a query that turns the columns into text first, whatever type they are stored as.
    // Nothing is ever written to these tables.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ReceivedTrans>(e =>
        {
            e.HasNoKey();
            e.ToSqlQuery("SELECT CAST(RenamedFile AS nvarchar(500)) AS RenamedFile, CAST(AssignTo AS nvarchar(100)) AS AssignTo FROM [Transact].[ReceivedTrans]");
        });

        builder.Entity<RingUser>(e =>
        {
            e.HasNoKey();
            e.ToSqlQuery("SELECT LTRIM(RTRIM(CAST(UserID AS nvarchar(100)))) AS UserID, LTRIM(RTRIM(CAST(Email AS nvarchar(500)))) AS Email FROM [Ring].[Users]");
        });

        builder.Entity<Subscriber>(e =>
        {
            e.HasNoKey();
            e.ToSqlQuery("SELECT LTRIM(RTRIM(CAST(ShortName AS nvarchar(100)))) AS ShortName, LTRIM(RTRIM(CAST(SubName AS nvarchar(500)))) AS SubName, LTRIM(RTRIM(CAST(SubCode AS nvarchar(100)))) AS SubCode, LTRIM(RTRIM(CAST(SubXDSCode AS nvarchar(100)))) AS SubXDSCode, LTRIM(RTRIM(CAST(SubCategoryCode AS nvarchar(100)))) AS SubCategoryCode FROM [Subscriber].[Subscribers]");
        });

        builder.Entity<SubscriberCategory>(e =>
        {
            e.HasNoKey();
            e.ToSqlQuery("SELECT LTRIM(RTRIM(CAST(SubCategoryCode AS nvarchar(100)))) AS SubCategoryCode, LTRIM(RTRIM(CAST(CatShortName AS nvarchar(200)))) AS CatShortName, LTRIM(RTRIM(CAST(CatDescription AS nvarchar(500)))) AS CatDescription FROM [Subscriber].[SubscriberCategory]");
        });
    }
}
