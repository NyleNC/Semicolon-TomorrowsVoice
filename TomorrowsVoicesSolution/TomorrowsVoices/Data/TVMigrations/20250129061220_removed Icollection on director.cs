using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class removedIcollectionondirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID1",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_DirectorID1",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DirectorID1",
                table: "Locations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectorID1",
                table: "Locations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DirectorID1",
                table: "Locations",
                column: "DirectorID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID1",
                table: "Locations",
                column: "DirectorID1",
                principalTable: "Directors",
                principalColumn: "ID");
        }
    }
}
