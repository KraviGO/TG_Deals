using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChannelCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Stage45_CatalogFiltersPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "catalog_channels",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "ru");

            migrationBuilder.AddColumn<decimal>(
                name: "price_per_post_rub",
                table: "catalog_channels",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 1000m);

            migrationBuilder.AddColumn<string>(
                name: "topic",
                table: "catalog_channels",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_language",
                table: "catalog_channels",
                column: "language");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_price_per_post_rub",
                table: "catalog_channels",
                column: "price_per_post_rub");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_channels_topic",
                table: "catalog_channels",
                column: "topic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_catalog_channels_language",
                table: "catalog_channels");

            migrationBuilder.DropIndex(
                name: "IX_catalog_channels_price_per_post_rub",
                table: "catalog_channels");

            migrationBuilder.DropIndex(
                name: "IX_catalog_channels_topic",
                table: "catalog_channels");

            migrationBuilder.DropColumn(
                name: "language",
                table: "catalog_channels");

            migrationBuilder.DropColumn(
                name: "price_per_post_rub",
                table: "catalog_channels");

            migrationBuilder.DropColumn(
                name: "topic",
                table: "catalog_channels");
        }
    }
}
