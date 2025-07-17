using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExtendingEpisodeRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Seasons",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Episodes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Episodes");
        }
    }
}
