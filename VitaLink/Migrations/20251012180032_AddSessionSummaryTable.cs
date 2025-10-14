using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionSummaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AthleteProfiles",
                columns: table => new
                {
                    AthleteID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyFatPercentage = table.Column<double>(type: "float", nullable: false),
                    TargetSport = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteProfiles", x => x.AthleteID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    AthleteID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.SessionID);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_AthleteProfiles_AthleteID",
                        column: x => x.AthleteID,
                        principalTable: "AthleteProfiles",
                        principalColumn: "AthleteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIRecommendations",
                columns: table => new
                {
                    RecommendationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FatigueLevel = table.Column<double>(type: "float", nullable: false),
                    OptimalIntensity = table.Column<double>(type: "float", nullable: false),
                    AlertMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRecommendations", x => x.RecommendationID);
                    table.ForeignKey(
                        name: "FK_AIRecommendations_TrainingSessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "TrainingSessions",
                        principalColumn: "SessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorDataRaw",
                columns: table => new
                {
                    DataID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    OxygenSaturation = table.Column<double>(type: "float", nullable: false),
                    BodyTemperature = table.Column<double>(type: "float", nullable: false),
                    SweatComposition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotionData_X = table.Column<double>(type: "float", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorDataRaw", x => x.DataID);
                    table.ForeignKey(
                        name: "FK_SensorDataRaw_TrainingSessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "TrainingSessions",
                        principalColumn: "SessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSummaries",
                columns: table => new
                {
                    SummaryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    AvgHeartRate = table.Column<double>(type: "float", nullable: false),
                    MaxHeartRate = table.Column<double>(type: "float", nullable: false),
                    CaloriesBurned = table.Column<double>(type: "float", nullable: false),
                    FatigueScore_Final = table.Column<double>(type: "float", nullable: false),
                    MaxMotionIntensity = table.Column<double>(type: "float", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSummaries", x => x.SummaryID);
                    table.ForeignKey(
                        name: "FK_SessionSummaries_TrainingSessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "TrainingSessions",
                        principalColumn: "SessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRecommendations_SessionID",
                table: "AIRecommendations",
                column: "SessionID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataRaw_SessionID",
                table: "SensorDataRaw",
                column: "SessionID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSummaries_SessionID",
                table: "SessionSummaries",
                column: "SessionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_AthleteID",
                table: "TrainingSessions",
                column: "AthleteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIRecommendations");

            migrationBuilder.DropTable(
                name: "SensorDataRaw");

            migrationBuilder.DropTable(
                name: "SessionSummaries");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "AthleteProfiles");
        }
    }
}
