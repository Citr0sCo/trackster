using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTraktApiIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CollectedAt",
                table: "MovieUserLinks",
                newName: "WatchedAt");

            migrationBuilder.RenameColumn(
                name: "CollectedAt",
                table: "EpisodeUserLinks",
                newName: "WatchedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WatchedAt",
                table: "MovieUserLinks",
                newName: "CollectedAt");

            migrationBuilder.RenameColumn(
                name: "WatchedAt",
                table: "EpisodeUserLinks",
                newName: "CollectedAt");
        }
    }
}
