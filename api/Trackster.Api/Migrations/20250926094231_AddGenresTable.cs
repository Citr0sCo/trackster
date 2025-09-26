using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGenresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    GenreIdentifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreIdentifier",
                        column: x => x.GenreIdentifier,
                        principalTable: "Genres",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieIdentifier",
                        column: x => x.MovieIdentifier,
                        principalTable: "Movies",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowGenres",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ShowIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    GenreIdentifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowGenres", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_ShowGenres_Genres_GenreIdentifier",
                        column: x => x.GenreIdentifier,
                        principalTable: "Genres",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowGenres_Shows_ShowIdentifier",
                        column: x => x.ShowIdentifier,
                        principalTable: "Shows",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreIdentifier",
                table: "MovieGenres",
                column: "GenreIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieIdentifier",
                table: "MovieGenres",
                column: "MovieIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_GenreIdentifier",
                table: "ShowGenres",
                column: "GenreIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_ShowIdentifier",
                table: "ShowGenres",
                column: "ShowIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "ShowGenres");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
