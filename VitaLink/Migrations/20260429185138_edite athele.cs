using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    public partial class editeathele : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "BodyFatPercentage",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "TargetSport",
                table: "AthleteProfiles");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AthleteProfiles");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AthleteProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "AthleteProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "BodyFatPercentage",
                table: "AthleteProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TargetSport",
                table: "AthleteProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "AthleteProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
