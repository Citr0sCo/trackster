using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateWebhookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Webhooks",
                columns: table => new
                {
                    Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    UserIdentifier = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Webhooks_Users_UserIdentifier",
                        column: x => x.UserIdentifier,
                        principalTable: "Users",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Webhooks_UserIdentifier",
                table: "Webhooks",
                column: "UserIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Webhooks");
        }
    }
}
