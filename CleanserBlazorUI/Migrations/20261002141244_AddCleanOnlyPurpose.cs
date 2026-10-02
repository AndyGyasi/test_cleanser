using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddCleanOnlyPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "DataLoggingCleanOnlyLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "DataLoggingCleanOnlyLogs");
        }
    }
}
