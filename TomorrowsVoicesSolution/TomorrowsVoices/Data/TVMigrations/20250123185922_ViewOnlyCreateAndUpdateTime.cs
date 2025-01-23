using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomorrowsVoices.Data.TVMigrations
{
    /// <inheritdoc />
    public partial class ViewOnlyCreateAndUpdateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Singers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Singers");
        }
    }
}
