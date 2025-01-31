using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubscriptionFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    target_branch = table.Column<string>(type: "text", nullable: true),
                    repository_name = table.Column<string>(type: "text", nullable: true),
                    team_name = table.Column<string>(type: "text", nullable: true),
                    reviewer_name = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionMetadata");

            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
