using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class UpdatedaddedemailUniquefordirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Directors_Email",
                table: "Directors",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Directors_Email",
                table: "Directors");
        }
    }
}
