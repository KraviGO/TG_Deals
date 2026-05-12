using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "publisher_ledger_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gross_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    platform_fee_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    publisher_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publisher_ledger_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "publisher_wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    available = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    paid_out = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publisher_wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "publisher_withdrawals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    withdrawal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    destination_card_mask = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publisher_withdrawals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    publisher_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "topups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    topup_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    yookassa_payment_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    confirmation_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wallet_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    tx_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    deal_id = table.Column<Guid>(type: "uuid", nullable: true),
                    topup_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    available = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    reserved = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "yookassa_webhook_inbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    event_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    yookassa_payment_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    remote_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yookassa_webhook_inbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_publisher_ledger_entries_deal_id",
                table: "publisher_ledger_entries",
                column: "deal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_publisher_ledger_entries_entry_id",
                table: "publisher_ledger_entries",
                column: "entry_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_publisher_ledger_entries_publisher_user_id",
                table: "publisher_ledger_entries",
                column: "publisher_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_publisher_ledger_entries_status",
                table: "publisher_ledger_entries",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_publisher_wallets_publisher_user_id",
                table: "publisher_wallets",
                column: "publisher_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_publisher_wallets_wallet_id",
                table: "publisher_wallets",
                column: "wallet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_publisher_withdrawals_publisher_user_id",
                table: "publisher_withdrawals",
                column: "publisher_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_publisher_withdrawals_withdrawal_id",
                table: "publisher_withdrawals",
                column: "withdrawal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_deal_id",
                table: "reservations",
                column: "deal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_publisher_user_id",
                table: "reservations",
                column: "publisher_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_reservation_id",
                table: "reservations",
                column: "reservation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_user_id",
                table: "reservations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_topups_topup_id",
                table: "topups",
                column: "topup_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topups_user_id",
                table: "topups",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_topups_yookassa_payment_id",
                table: "topups",
                column: "yookassa_payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_deal_id",
                table: "wallet_transactions",
                column: "deal_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_topup_id",
                table: "wallet_transactions",
                column: "topup_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_tx_id",
                table: "wallet_transactions",
                column: "tx_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_transactions_user_id",
                table: "wallet_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_user_id",
                table: "wallets",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallets_wallet_id",
                table: "wallets",
                column: "wallet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_yookassa_webhook_inbox_message_id",
                table: "yookassa_webhook_inbox",
                column: "message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_yookassa_webhook_inbox_processed_at",
                table: "yookassa_webhook_inbox",
                column: "processed_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "publisher_ledger_entries");

            migrationBuilder.DropTable(
                name: "publisher_wallets");

            migrationBuilder.DropTable(
                name: "publisher_withdrawals");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "topups");

            migrationBuilder.DropTable(
                name: "wallet_transactions");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "yookassa_webhook_inbox");
        }
    }
}
