using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocalSubscriberTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Subscriber names, codes and categories are read LIVE from XDSDataLogDB (Subscriber.Subscribers and
            // Subscriber.SubscriberCategory); the app no longer keeps local copies.

            // 1. Unloadable log headers keep the subscriber's short code instead of pointing at a local profile row.
            migrationBuilder.AddColumn<string>(
                name: "SubscriberCode",
                table: "UnloadableLogHeaders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            //    Carry the code over for any header that already exists (so history is not lost).
            migrationBuilder.Sql(@"
                UPDATE h SET h.SubscriberCode = p.SubscriberCode
                FROM UnloadableLogHeaders h
                JOIN SubscriberProfiles p ON p.Id = h.SubscriberProfileId;");

            migrationBuilder.DropForeignKey(
                name: "FK_UnloadableLogHeaders_SubscriberProfiles_SubscriberProfileId",
                table: "UnloadableLogHeaders");

            migrationBuilder.DropIndex(
                name: "IX_UnloadableLogHeaders_SubscriberProfileId",
                table: "UnloadableLogHeaders");

            migrationBuilder.DropColumn(
                name: "SubscriberProfileId",
                table: "UnloadableLogHeaders");

            // 2. The two local tables go.
            migrationBuilder.DropTable(
                name: "SubscriberProfiles");

            migrationBuilder.DropTable(
                name: "SubscriberShortCodes");

            // 3. A hand-made copy of Subscriber.Subscribers inside THIS app's database (not part of the EF model)
            //    is removed too. This runs on the application database connection only, and it refuses to run if
            //    the database it is connected to is XDSDataLogDB: the mother database is never touched.
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[Subscriber].[Subscribers]', N'U') IS NOT NULL AND DB_NAME() NOT LIKE N'XDSDataLog%'
                    DROP TABLE [Subscriber].[Subscribers];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // (Going back restores the empty tables only; the manually made Subscriber.Subscribers copy is not recreated.)
            migrationBuilder.DropColumn(
                name: "SubscriberCode",
                table: "UnloadableLogHeaders");

            migrationBuilder.AddColumn<int>(
                name: "SubscriberProfileId",
                table: "UnloadableLogHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubscriberProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstitutionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriberCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriberName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberShortCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactEmail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSubscribe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Position1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubBoGCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubXDSCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberShortCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnloadableLogHeaders_SubscriberProfileId",
                table: "UnloadableLogHeaders",
                column: "SubscriberProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberProfiles_SubscriberCode",
                table: "SubscriberProfiles",
                column: "SubscriberCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UnloadableLogHeaders_SubscriberProfiles_SubscriberProfileId",
                table: "UnloadableLogHeaders",
                column: "SubscriberProfileId",
                principalTable: "SubscriberProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
