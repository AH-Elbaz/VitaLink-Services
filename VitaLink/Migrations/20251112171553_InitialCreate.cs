using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw");

            migrationBuilder.AlterColumn<string>(
                name: "BeltID",
                table: "SensorDataRaw",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw");

            migrationBuilder.AlterColumn<string>(
                name: "BeltID",
                table: "SensorDataRaw",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw",
                column: "BeltID");
        }
    }
}
