using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class AddedSomeFieldsToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleID",
                table: "VolAttendances",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ActualEndTime",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ActualStartTime",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolAttendances_ScheduleID",
                table: "VolAttendances",
                column: "ScheduleID");

            migrationBuilder.AddForeignKey(
                name: "FK_VolAttendances_Schedules_ScheduleID",
                table: "VolAttendances",
                column: "ScheduleID",
                principalTable: "Schedules",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolAttendances_Schedules_ScheduleID",
                table: "VolAttendances");

            migrationBuilder.DropIndex(
                name: "IX_VolAttendances_ScheduleID",
                table: "VolAttendances");

            migrationBuilder.DropColumn(
                name: "ScheduleID",
                table: "VolAttendances");

            migrationBuilder.DropColumn(
                name: "ActualEndTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ActualStartTime",
                table: "Schedules");
        }
    }
}
