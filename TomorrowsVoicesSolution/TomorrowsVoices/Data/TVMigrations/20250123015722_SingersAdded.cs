using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class SingersAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Singers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Singers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "City",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations",
                column: "DirectorID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Singers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Singers");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Locations",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DirectorID",
                table: "Locations",
                column: "DirectorID");
        }
    }
}
