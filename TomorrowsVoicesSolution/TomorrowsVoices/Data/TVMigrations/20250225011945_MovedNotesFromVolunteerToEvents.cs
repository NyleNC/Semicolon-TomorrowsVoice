using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class MovedNotesFromVolunteerToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VolAttendances");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Events",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VolAttendances",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }
    }
}
