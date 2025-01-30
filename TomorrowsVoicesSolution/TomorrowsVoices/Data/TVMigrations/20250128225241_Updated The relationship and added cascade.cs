using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class UpdatedTherelationshipandaddedcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID");
        }
    }
}
