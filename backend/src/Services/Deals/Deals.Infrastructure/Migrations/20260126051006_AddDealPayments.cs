using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDealPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "payment_id",
                table: "deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_state",
                table: "deals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_id",
                table: "deals");

            migrationBuilder.DropColumn(
                name: "payment_state",
                table: "deals");
        }
    }
}
