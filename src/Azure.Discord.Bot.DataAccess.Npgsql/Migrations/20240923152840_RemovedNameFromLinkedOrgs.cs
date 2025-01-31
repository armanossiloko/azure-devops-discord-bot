using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNameFromLinkedOrgs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "linked_server_organizations",
                newName: "display_name");

            migrationBuilder.AddColumn<long>(
                name: "organization_id",
                table: "subscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "project_id",
                table: "subscription_metadata",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_organization_id",
                table: "subscriptions",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "FK_subscriptions_linked_server_organizations_organization_id",
                table: "subscriptions",
                column: "organization_id",
                principalTable: "linked_server_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_linked_server_organizations_organization_id",
                table: "subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_subscriptions_organization_id",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "project_id",
                table: "subscription_metadata");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "linked_server_organizations",
                newName: "name");
        }
    }
}
