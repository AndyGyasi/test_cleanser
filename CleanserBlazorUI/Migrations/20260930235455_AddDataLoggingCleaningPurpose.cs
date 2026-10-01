using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddDataLoggingCleaningPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataLoggingCleaningPurposeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubXDSCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingCleaningPurposeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataLoggingCleaningPurposeReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingCleaningPurposeReasons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataLoggingCleaningPurposeLogs");

            migrationBuilder.DropTable(
                name: "DataLoggingCleaningPurposeReasons");
        }
    }
}
