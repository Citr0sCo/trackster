using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserThirdPartyIntegrationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThirdPartyIntegrations",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserRecordIdentifier = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdPartyIntegrations", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_ThirdPartyIntegrations_Users_UserRecordIdentifier",
                        column: x => x.UserRecordIdentifier,
                        principalTable: "Users",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThirdPartyIntegrations_UserRecordIdentifier",
                table: "ThirdPartyIntegrations",
                column: "UserRecordIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThirdPartyIntegrations");
        }
    }
}
