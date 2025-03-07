using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class addedEventonSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Events_EventID",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "Schedules",
                newName: "eventID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_EventID",
                table: "Schedules",
                newName: "IX_Schedules_eventID");

            migrationBuilder.AlterColumn<int>(
                name: "eventID",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Events_eventID",
                table: "Schedules",
                column: "eventID",
                principalTable: "Events",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Events_eventID",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "eventID",
                table: "Schedules",
                newName: "EventID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_eventID",
                table: "Schedules",
                newName: "IX_Schedules_EventID");

            migrationBuilder.AlterColumn<int>(
                name: "EventID",
                table: "Schedules",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Events_EventID",
                table: "Schedules",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "ID");
        }
    }
}
