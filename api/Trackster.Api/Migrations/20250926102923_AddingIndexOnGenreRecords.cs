using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexOnGenreRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Genres_GenreIdentifier",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Movies_MovieIdentifier",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGenres_Genres_GenreIdentifier",
                table: "ShowGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGenres_Shows_ShowIdentifier",
                table: "ShowGenres");

            migrationBuilder.DropIndex(
                name: "IX_ShowGenres_ShowIdentifier",
                table: "ShowGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MovieIdentifier",
                table: "MovieGenres");

            migrationBuilder.RenameColumn(
                name: "ShowIdentifier",
                table: "ShowGenres",
                newName: "ShowId");

            migrationBuilder.RenameColumn(
                name: "GenreIdentifier",
                table: "ShowGenres",
                newName: "GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_ShowGenres_GenreIdentifier",
                table: "ShowGenres",
                newName: "IX_ShowGenres_GenreId");

            migrationBuilder.RenameColumn(
                name: "MovieIdentifier",
                table: "MovieGenres",
                newName: "MovieId");

            migrationBuilder.RenameColumn(
                name: "GenreIdentifier",
                table: "MovieGenres",
                newName: "GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieGenres_GenreIdentifier",
                table: "MovieGenres",
                newName: "IX_MovieGenres_GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_ShowId_GenreId",
                table: "ShowGenres",
                columns: new[] { "ShowId", "GenreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieId_GenreId",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGenres_Genres_GenreId",
                table: "ShowGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGenres_Shows_ShowId",
                table: "ShowGenres",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGenres_Genres_GenreId",
                table: "ShowGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGenres_Shows_ShowId",
                table: "ShowGenres");

            migrationBuilder.DropIndex(
                name: "IX_ShowGenres_ShowId_GenreId",
                table: "ShowGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MovieId_GenreId",
                table: "MovieGenres");

            migrationBuilder.RenameColumn(
                name: "ShowId",
                table: "ShowGenres",
                newName: "ShowIdentifier");

            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "ShowGenres",
                newName: "GenreIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_ShowGenres_GenreId",
                table: "ShowGenres",
                newName: "IX_ShowGenres_GenreIdentifier");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "MovieGenres",
                newName: "MovieIdentifier");

            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "MovieGenres",
                newName: "GenreIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                newName: "IX_MovieGenres_GenreIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_ShowIdentifier",
                table: "ShowGenres",
                column: "ShowIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieIdentifier",
                table: "MovieGenres",
                column: "MovieIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreIdentifier",
                table: "MovieGenres",
                column: "GenreIdentifier",
                principalTable: "Genres",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Movies_MovieIdentifier",
                table: "MovieGenres",
                column: "MovieIdentifier",
                principalTable: "Movies",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGenres_Genres_GenreIdentifier",
                table: "ShowGenres",
                column: "GenreIdentifier",
                principalTable: "Genres",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGenres_Shows_ShowIdentifier",
                table: "ShowGenres",
                column: "ShowIdentifier",
                principalTable: "Shows",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
