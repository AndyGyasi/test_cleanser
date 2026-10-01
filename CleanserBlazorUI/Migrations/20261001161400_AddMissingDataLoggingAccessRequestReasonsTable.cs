using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingDataLoggingAccessRequestReasonsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hand-written: the model snapshot already recorded this table (it was
            // added to the DbContext at the same time as AddDataLoggingAccessRequestsAndUnloggedAttempts
            // but its CreateTable call was missing from that migration's Up()), so
            // `dotnet ef migrations add` sees no model diff and scaffolds empty here.
            migrationBuilder.CreateTable(
                name: "DataLoggingAccessRequestReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingAccessRequestReasons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataLoggingAccessRequestReasons");
        }
    }
}
