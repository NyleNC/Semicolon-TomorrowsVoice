using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class addedthemodelbuilders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules",
                column: "volLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules",
                column: "volLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
