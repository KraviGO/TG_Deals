using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChannelCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "catalog_channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_channel_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    intake_mode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ownership_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_channels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_channel_id",
                table: "catalog_channels",
                column: "channel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_intake_mode",
                table: "catalog_channels",
                column: "intake_mode");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_ownership_status",
                table: "catalog_channels",
                column: "ownership_status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalog_channels");
        }
    }
}
