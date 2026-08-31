using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSubscriberShortCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail1",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail2",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail3",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName1",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName2",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName3",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone1",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone2",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone3",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "SubscriberShortCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSubscribe",
                table: "SubscriberShortCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivatedReason",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryDate",
                table: "SubscriberShortCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position1",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position2",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position3",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostAdd",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectorCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubBoGCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubName",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubStatus",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubXDSCode",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransDate",
                table: "SubscriberShortCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "SubscriberShortCodes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail1",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactEmail2",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactEmail3",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactName1",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactName2",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactName3",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactPhone1",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactPhone2",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "ContactPhone3",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "DateSubscribe",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "DeactivatedReason",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Position1",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Position2",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Position3",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "PostAdd",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "RegionCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SectorCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SubBoGCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SubCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SubName",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SubStatus",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "SubXDSCode",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "TransDate",
                table: "SubscriberShortCodes");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "SubscriberShortCodes");
        }
    }
}
