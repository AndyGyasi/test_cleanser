using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddCleanOnlyLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataLoggingCleanOnlyLogs",
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
                    PerformedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileInReceivedTrans = table.Column<bool>(type: "bit", nullable: false),
                    AssignedToEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessBasis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingCleanOnlyLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataLoggingCleanOnlyLogs");
        }
    }
}
