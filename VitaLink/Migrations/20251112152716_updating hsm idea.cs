using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Migrations
{
    /// <inheritdoc />
    public partial class updatinghsmidea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorDataRaw_TrainingSessions_SessionID",
                table: "SensorDataRaw");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw");

            migrationBuilder.DropIndex(
                name: "IX_SensorDataRaw_SessionID",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "DataID",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "BodyTemperature",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "MotionData_X",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "OxygenSaturation",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "SweatComposition",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "SensorDataRaw");

            migrationBuilder.RenameColumn(
                name: "SessionID",
                table: "SensorDataRaw",
                newName: "Sweat");

            migrationBuilder.AlterColumn<float>(
                name: "HeartRate",
                table: "SensorDataRaw",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BeltID",
                table: "SensorDataRaw",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "AccX",
                table: "SensorDataRaw",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AccY",
                table: "SensorDataRaw",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AccZ",
                table: "SensorDataRaw",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<byte>(
                name: "Spo2",
                table: "SensorDataRaw",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<float>(
                name: "Temperature",
                table: "SensorDataRaw",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "TrainingSessionSessionID",
                table: "SensorDataRaw",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw",
                column: "BeltID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataRaw_TrainingSessionSessionID",
                table: "SensorDataRaw",
                column: "TrainingSessionSessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorDataRaw_TrainingSessions_TrainingSessionSessionID",
                table: "SensorDataRaw",
                column: "TrainingSessionSessionID",
                principalTable: "TrainingSessions",
                principalColumn: "SessionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorDataRaw_TrainingSessions_TrainingSessionSessionID",
                table: "SensorDataRaw");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw");

            migrationBuilder.DropIndex(
                name: "IX_SensorDataRaw_TrainingSessionSessionID",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "BeltID",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "AccX",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "AccY",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "AccZ",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "Spo2",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "SensorDataRaw");

            migrationBuilder.DropColumn(
                name: "TrainingSessionSessionID",
                table: "SensorDataRaw");

            migrationBuilder.RenameColumn(
                name: "Sweat",
                table: "SensorDataRaw",
                newName: "SessionID");

            migrationBuilder.AlterColumn<int>(
                name: "HeartRate",
                table: "SensorDataRaw",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<long>(
                name: "DataID",
                table: "SensorDataRaw",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<double>(
                name: "BodyTemperature",
                table: "SensorDataRaw",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MotionData_X",
                table: "SensorDataRaw",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OxygenSaturation",
                table: "SensorDataRaw",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "SweatComposition",
                table: "SensorDataRaw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "SensorDataRaw",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorDataRaw",
                table: "SensorDataRaw",
                column: "DataID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataRaw_SessionID",
                table: "SensorDataRaw",
                column: "SessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorDataRaw_TrainingSessions_SessionID",
                table: "SensorDataRaw",
                column: "SessionID",
                principalTable: "TrainingSessions",
                principalColumn: "SessionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
