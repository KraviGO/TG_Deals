using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Publishers.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChannelVerificationChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verification_code",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "verification_expires_at",
                table: "channels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "verification_code",
                table: "channels",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "verification_expires_at",
                table: "channels",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
