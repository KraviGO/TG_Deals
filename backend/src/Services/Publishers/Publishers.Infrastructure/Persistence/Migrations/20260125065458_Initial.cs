using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publishers.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_channel_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    intake_mode = table.Column<int>(type: "integer", nullable: false),
                    ownership_status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    verification_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    verification_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_channels_channel_id",
                table: "channels",
                column: "channel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_channels_publisher_user_id_telegram_channel_id",
                table: "channels",
                columns: new[] { "publisher_user_id", "telegram_channel_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channels");
        }
    }
}
