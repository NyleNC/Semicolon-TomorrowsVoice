using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class addedDirectorLocationClassmanyTomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DirectorID",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Directors");

            migrationBuilder.CreateTable(
                name: "DirectorLocations",
                columns: table => new
                {
                    DirectorID = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectorLocations", x => new { x.DirectorID, x.LocationID });
                    table.ForeignKey(
                        name: "FK_DirectorLocations_Directors_DirectorID",
                        column: x => x.DirectorID,
                        principalTable: "Directors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectorLocations_Locations_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectorLocations_LocationID",
                table: "DirectorLocations",
                column: "LocationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectorLocations");

            migrationBuilder.AddColumn<int>(
                name: "DirectorID",
                table: "Locations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Directors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations",
                column: "DirectorID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
