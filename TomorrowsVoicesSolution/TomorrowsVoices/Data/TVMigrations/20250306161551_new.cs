using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Schedules");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "volLocationID",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_volLocationID",
                table: "Schedules",
                column: "volLocationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules",
                column: "volLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_volLocationID",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "volLocationID",
                table: "Schedules");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Schedules",
                type: "TEXT",
                nullable: true);
        }
    }
}
