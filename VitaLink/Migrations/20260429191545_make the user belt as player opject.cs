using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    public partial class maketheuserbeltasplayeropject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "UserBelts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "UserBelts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "BodyFatPercentage",
                table: "UserBelts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImage",
                table: "UserBelts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetSport",
                table: "UserBelts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "UserBelts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "UserBelts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "BodyFatPercentage",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "TargetSport",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "UserBelts");

            migrationBuilder.DropColumn(
                name: "name",
                table: "UserBelts");
        }
    }
}
