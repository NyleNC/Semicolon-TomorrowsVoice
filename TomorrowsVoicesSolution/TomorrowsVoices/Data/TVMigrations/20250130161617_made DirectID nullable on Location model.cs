using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class madeDirectIDnullableonLocationmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.AlterColumn<int>(
                name: "DirectorID",
                table: "Locations",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Directors_DirectorID",
                table: "Locations");

            migrationBuilder.AlterColumn<int>(
                name: "DirectorID",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

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
