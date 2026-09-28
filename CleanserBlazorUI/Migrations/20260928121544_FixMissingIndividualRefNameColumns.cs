using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanserBlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingIndividualRefNameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // sprint10_ref_name_fields shipped as a no-op, assuming Surname/FirstName
            // had already been added by hand to every database -- true for the
            // original DB it was written against, false for databases created fresh
            // since (e.g. a new environment's IndividualsData table never got them),
            // which then fail at runtime with "Invalid column name 'FirstName'"/
            // "'Surname'". Guarded so this is a no-op wherever the columns are
            // already present, and adds them wherever they're actually missing.
            migrationBuilder.Sql(@"
IF COL_LENGTH('IndividualsData', 'Surname') IS NULL
BEGIN
    ALTER TABLE [IndividualsData] ADD [Surname] nvarchar(max) NULL;
END
IF COL_LENGTH('IndividualsData', 'FirstName') IS NULL
BEGIN
    ALTER TABLE [IndividualsData] ADD [FirstName] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Columns retained on rollback -- removing them could cause data loss,
            // matching sprint10_ref_name_fields' own Down().
        }
    }
}
