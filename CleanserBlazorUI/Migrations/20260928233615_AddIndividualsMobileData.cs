using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddIndividualsMobileData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndividualsMobileData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrenVersion = table.Column<int>(type: "int", nullable: false),
                    SubscriberCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditFacilityAccNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisbursementDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastConfirmedReportingPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatIDNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VotersIDNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverLicNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SSNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EzwichNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherIDNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacilityStatusCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualsMobileData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndividualsMobileData");
        }
    }
}
