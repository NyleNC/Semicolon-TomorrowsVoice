using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class addedApprovalForVolunteerRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApproveID",
                table: "Volunteers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Volunteers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveID",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Volunteers");
        }
    }
}
