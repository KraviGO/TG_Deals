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
                name: "deal_disputes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    dispute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opened_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opened_by_role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolution_action = table.Column<int>(type: "integer", nullable: true),
                    resolution_note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deal_disputes", x => x.Id);
                });

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
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    funding_status = table.Column<int>(type: "integer", nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    post_url = table.Column<string>(type: "text", nullable: true),
                    published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    publisher_comment = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deal_disputes_deal_id",
                table: "deal_disputes",
                column: "deal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deal_disputes_dispute_id",
                table: "deal_disputes",
                column: "dispute_id",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_deals_reservation_id",
                table: "deals",
                column: "reservation_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deal_disputes");

            migrationBuilder.DropTable(
                name: "deals");
        }
    }
}
