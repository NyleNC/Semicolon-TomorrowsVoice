using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class AddedonDeleteNoactonforSingerAttendanceandSessionthenaddedRestrctforLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Singers_SingerID",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Singers_Locations_LocationID",
                table: "Singers");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Singers_SingerID",
                table: "Attendances",
                column: "SingerID",
                principalTable: "Singers",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Singers_Locations_LocationID",
                table: "Singers",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Singers_SingerID",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Singers_Locations_LocationID",
                table: "Singers");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Singers_SingerID",
                table: "Attendances",
                column: "SingerID",
                principalTable: "Singers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Locations_LocationID",
                table: "Sessions",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Singers_Locations_LocationID",
                table: "Singers",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
