using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedSubscriptionFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionMetadata");

            migrationBuilder.CreateTable(
                name: "subscription_metadata",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    target_branch = table.Column<string>(type: "text", nullable: true),
                    repository_name = table.Column<string>(type: "text", nullable: true),
                    team_name = table.Column<string>(type: "text", nullable: true),
                    reviewer_name = table.Column<string>(type: "text", nullable: true),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_metadata");

            migrationBuilder.CreateTable(
                name: "SubscriptionMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    repository_name = table.Column<string>(type: "text", nullable: true),
                    reviewer_name = table.Column<string>(type: "text", nullable: true),
                    target_branch = table.Column<string>(type: "text", nullable: true),
                    team_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionMetadata_subscriptions_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionMetadata_OwnerId",
                table: "SubscriptionMetadata",
                column: "OwnerId");
        }
    }
}
