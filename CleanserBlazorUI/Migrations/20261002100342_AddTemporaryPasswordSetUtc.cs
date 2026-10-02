using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporaryPasswordSetUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TemporaryPasswordSetUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            // Accounts that are already waiting to set their own password get their
            // 3 days starting now, rather than being open-ended.
            migrationBuilder.Sql(@"
                UPDATE u SET u.TemporaryPasswordSetUtc = SYSUTCDATETIME()
                FROM AspNetUsers u
                WHERE EXISTS (SELECT 1 FROM AspNetUserRoles ur
                              JOIN AspNetRoles r ON r.Id = ur.RoleId
                              WHERE ur.UserId = u.Id AND r.Name = 'MustChangePassword');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryPasswordSetUtc",
                table: "AspNetUsers");
        }
    }
}
