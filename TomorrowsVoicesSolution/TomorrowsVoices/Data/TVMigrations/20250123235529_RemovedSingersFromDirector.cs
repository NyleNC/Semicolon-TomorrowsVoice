using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class RemovedSingersFromDirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Singers_Directors_DirectorID",
                table: "Singers");

            migrationBuilder.DropIndex(
                name: "IX_Singers_DirectorID",
                table: "Singers");

            migrationBuilder.DropColumn(
                name: "DirectorID",
                table: "Singers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectorID",
                table: "Singers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Singers_DirectorID",
                table: "Singers",
                column: "DirectorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Singers_Directors_DirectorID",
                table: "Singers",
                column: "DirectorID",
                principalTable: "Directors",
                principalColumn: "ID");
        }
    }
}
