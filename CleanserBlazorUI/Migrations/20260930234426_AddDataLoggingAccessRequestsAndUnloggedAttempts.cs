using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddDataLoggingAccessRequestsAndUnloggedAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataLoggingAccessRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubXDSCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingAccessRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataLoggingUnloggedAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastAttemptedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstAttemptedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAttemptedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    DataProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubXDSCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggingUnloggedAttempts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataLoggingUnloggedAttempts_Filename",
                table: "DataLoggingUnloggedAttempts",
                column: "Filename",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataLoggingAccessRequests");

            migrationBuilder.DropTable(
                name: "DataLoggingUnloggedAttempts");
        }
    }
}
