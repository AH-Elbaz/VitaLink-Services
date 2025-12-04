using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class AddAzurePersonId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AzurePersonId",
                table: "AthleteProfiles",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzurePersonId",
                table: "AthleteProfiles");
        }
    }
}
