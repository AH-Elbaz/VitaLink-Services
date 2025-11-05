using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class AddBeltIdToAthlete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeltID",
                table: "AthleteProfiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteProfiles_BeltID",
                table: "AthleteProfiles",
                column: "BeltID",
                unique: true,
                filter: "[BeltID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AthleteProfiles_BeltID",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "BeltID",
                table: "AthleteProfiles");
        }
    }
}
