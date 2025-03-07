using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class newmigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "volLocationID",
                table: "Schedules",
                newName: "VolLocationID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_volLocationID",
                table: "Schedules",
                newName: "IX_Schedules_VolLocationID");

            migrationBuilder.AlterColumn<int>(
                name: "VolLocationID",
                table: "Schedules",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_VolLocationID",
                table: "Schedules",
                column: "VolLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_VolLocations_VolLocationID",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "VolLocationID",
                table: "Schedules",
                newName: "volLocationID");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_VolLocationID",
                table: "Schedules",
                newName: "IX_Schedules_volLocationID");

            migrationBuilder.AlterColumn<int>(
                name: "volLocationID",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_VolLocations_volLocationID",
                table: "Schedules",
                column: "volLocationID",
                principalTable: "VolLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
