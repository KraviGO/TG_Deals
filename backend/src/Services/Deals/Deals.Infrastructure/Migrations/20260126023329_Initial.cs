using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    deal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    advertiser_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_text = table.Column<string>(type: "text", nullable: false),
                    desired_publish_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deals_advertiser_user_id",
                table: "deals",
                column: "advertiser_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_deals_channel_id",
                table: "deals",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_deals_deal_id",
                table: "deals",
                column: "deal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deals_publisher_user_id",
                table: "deals",
                column: "publisher_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deals");
        }
    }
}
