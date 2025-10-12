using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingExplicitUserReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyIntegrations_Users_UserRecordIdentifier",
                table: "ThirdPartyIntegrations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserRecordIdentifier",
                table: "ThirdPartyIntegrations",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyIntegrations_Users_UserRecordIdentifier",
                table: "ThirdPartyIntegrations",
                column: "UserRecordIdentifier",
                principalTable: "Users",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyIntegrations_Users_UserRecordIdentifier",
                table: "ThirdPartyIntegrations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserRecordIdentifier",
                table: "ThirdPartyIntegrations",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyIntegrations_Users_UserRecordIdentifier",
                table: "ThirdPartyIntegrations",
                column: "UserRecordIdentifier",
                principalTable: "Users",
                principalColumn: "Identifier");
        }
    }
}
