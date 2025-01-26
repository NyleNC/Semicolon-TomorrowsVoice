using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class SessionNotesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Sessions",
                type: "TEXT",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 2000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Sessions",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
