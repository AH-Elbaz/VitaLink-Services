using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class converttherelationbetweenthebeltanduserfromOTOtoOTM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AthleteProfiles_BeltID",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "BeltID",
                table: "AthleteProfiles");

            migrationBuilder.CreateTable(
                name: "UserBelts",
                columns: table => new
                {
                    BeltID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AthleteID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBelts", x => x.BeltID);
                    table.ForeignKey(
                        name: "FK_UserBelts_AthleteProfiles_AthleteID",
                        column: x => x.AthleteID,
                        principalTable: "AthleteProfiles",
                        principalColumn: "AthleteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBelts_AthleteID",
                table: "UserBelts",
                column: "AthleteID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBelts_BeltID",
                table: "UserBelts",
                column: "BeltID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBelts");

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
    }
}
