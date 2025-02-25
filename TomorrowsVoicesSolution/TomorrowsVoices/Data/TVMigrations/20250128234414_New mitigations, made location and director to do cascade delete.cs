using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class NewmitigationsmadelocationanddirectortodocascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
