using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSensorDataSchema : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SensorDataRaw",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "SensorDataRaw",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "Timestamp",
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
