using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingMovieRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TMDB",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Movies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "MovieUserLinks",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieIdentifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieUserLinks", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_MovieUserLinks_Movies_MovieIdentifier",
                        column: x => x.MovieIdentifier,
                        principalTable: "Movies",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieUserLinks_Users_UserIdentifier",
                        column: x => x.UserIdentifier,
                        principalTable: "Users",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieUserLinks_MovieIdentifier",
                table: "MovieUserLinks",
                column: "MovieIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_MovieUserLinks_UserIdentifier",
                table: "MovieUserLinks",
                column: "UserIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieUserLinks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "TMDB",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Movies");
        }
    }
}
