using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedSubscriptionsAndFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_metadata");

            migrationBuilder.AddColumn<string>(
                name: "project_id",
                table: "subscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "repository_name",
                table: "subscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "reviewer_names",
                table: "subscriptions",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "target_branch",
                table: "subscriptions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "project_id",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "repository_name",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "reviewer_names",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "target_branch",
                table: "subscriptions");

            migrationBuilder.CreateTable(
                name: "subscription_metadata",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<string>(type: "text", nullable: true),
                    repository_name = table.Column<string>(type: "text", nullable: true),
                    reviewer_name = table.Column<string>(type: "text", nullable: true),
                    target_branch = table.Column<string>(type: "text", nullable: true),
                    team_name = table.Column<string>(type: "text", nullable: true),
                    subscription_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_metadata", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_metadata_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_metadata_subscription_id",
                table: "subscription_metadata",
                column: "subscription_id");
        }
    }
}
