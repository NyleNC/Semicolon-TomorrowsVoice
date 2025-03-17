using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class EntireNewApproach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Events_eventID",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_VolLocationID",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Volunteers_volunteerID",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_VolAttendances_Events_EventID",
                table: "VolAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "VolAttendances");

            migrationBuilder.DropColumn(
                name: "ScheduledEndTime",
                table: "VolAttendances");

            migrationBuilder.DropColumn(
                name: "ScheduledStartTime",
                table: "VolAttendances");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "VolAttendances",
                newName: "VolScheduleID");

            migrationBuilder.RenameColumn(
                name: "ActualStartTime",
                table: "VolAttendances",
                newName: "ActualStart");

            migrationBuilder.RenameColumn(
                name: "ActualEndTime",
                table: "VolAttendances",
                newName: "ActualEnd");

            migrationBuilder.RenameIndex(
                name: "IX_VolAttendances_EventID",
                table: "VolAttendances",
                newName: "IX_VolAttendances_VolScheduleID");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Events",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Events",
                newName: "End");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_volunteerID",
                table: "Schedule",
                newName: "IX_Schedule_volunteerID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_VolLocationID",
                table: "Schedule",
                newName: "IX_Schedule_VolLocationID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_eventID",
                table: "Schedule",
                newName: "IX_Schedule_eventID");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Events",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "VolSchedules",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduledStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolSchedules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VolSchedules_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolSchedules_EventID",
                table: "VolSchedules",
                column: "EventID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Events_eventID",
                table: "Schedule",
                column: "eventID",
                principalTable: "Events",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_VolLocations_VolLocationID",
                table: "Schedule",
                column: "VolLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Volunteers_volunteerID",
                table: "Schedule",
                column: "volunteerID",
                principalTable: "Volunteers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VolAttendances_VolSchedules_VolScheduleID",
                table: "VolAttendances",
                column: "VolScheduleID",
                principalTable: "VolSchedules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Events_eventID",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_VolLocations_VolLocationID",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Volunteers_volunteerID",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_VolAttendances_VolSchedules_VolScheduleID",
                table: "VolAttendances");

            migrationBuilder.DropTable(
                name: "VolSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "Schedules");

            migrationBuilder.RenameColumn(
                name: "VolScheduleID",
                table: "VolAttendances",
                newName: "EventID");

            migrationBuilder.RenameColumn(
                name: "ActualStart",
                table: "VolAttendances",
                newName: "ActualStartTime");

            migrationBuilder.RenameColumn(
                name: "ActualEnd",
                table: "VolAttendances",
                newName: "ActualEndTime");

            migrationBuilder.RenameIndex(
                name: "IX_VolAttendances_VolScheduleID",
                table: "VolAttendances",
                newName: "IX_VolAttendances_EventID");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Events",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Events",
                newName: "EndTime");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_volunteerID",
                table: "Schedules",
                newName: "IX_Schedules_volunteerID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_VolLocationID",
                table: "Schedules",
                newName: "IX_Schedules_VolLocationID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_eventID",
                table: "Schedules",
                newName: "IX_Schedules_eventID");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "VolAttendances",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledEndTime",
                table: "VolAttendances",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledStartTime",
                table: "VolAttendances",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Events",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Events_eventID",
                table: "Schedules",
                column: "eventID",
                principalTable: "Events",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_VolLocationID",
                table: "Schedules",
                column: "VolLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Volunteers_volunteerID",
                table: "Schedules",
                column: "volunteerID",
                principalTable: "Volunteers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VolAttendances_Events_EventID",
                table: "VolAttendances",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
