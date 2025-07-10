using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingShowRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TvShowRecord");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    TMDB = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowIdentifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Seasons_Shows_ShowIdentifier",
                        column: x => x.ShowIdentifier,
                        principalTable: "Shows",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonIdentifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Episodes_Seasons_SeasonIdentifier",
                        column: x => x.SeasonIdentifier,
                        principalTable: "Seasons",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeUserLinks",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    EpisodeIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeUserLinks", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_EpisodeUserLinks_Episodes_EpisodeIdentifier",
                        column: x => x.EpisodeIdentifier,
                        principalTable: "Episodes",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpisodeUserLinks_Users_UserIdentifier",
                        column: x => x.UserIdentifier,
                        principalTable: "Users",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SeasonIdentifier",
                table: "Episodes",
                column: "SeasonIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeUserLinks_EpisodeIdentifier",
                table: "EpisodeUserLinks",
                column: "EpisodeIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeUserLinks_UserIdentifier",
                table: "EpisodeUserLinks",
                column: "UserIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_ShowIdentifier",
                table: "Seasons",
                column: "ShowIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpisodeUserLinks");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "TvShowRecord",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowRecord", x => x.Identifier);
                });
        }
    }
}
