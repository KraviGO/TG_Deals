using Payments.Entities.TopUps;
using Payments.Entities.Webhooks;
using PublisherLedgerEntryEntity = Payments.Entities.PublisherLedger.PublisherLedgerEntry;
using PublisherWithdrawalEntity = Payments.Entities.PublisherLedger.PublisherWithdrawal;
using PublisherWalletEntity = Payments.Entities.PublisherLedger.PublisherWallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using ReservationEntity = Payments.Entities.Wallet.Reservation;
using WalletTransactionEntity = Payments.Entities.Wallet.WalletTransaction;

namespace Payments.UseCases.Abstractions.Persistence;

public interface IPaymentsDbContext
{

    IQueryable<WalletEntity> Wallets { get; }
    IQueryable<ReservationEntity> Reservations { get; }
    IQueryable<WalletTransactionEntity> WalletTransactions { get; }
    IQueryable<TopUp> TopUps { get; }
    IQueryable<YooKassaWebhookInboxMessage> YooKassaWebhookInboxMessages { get; }
    IQueryable<PublisherWalletEntity> PublisherWallets { get; }
    IQueryable<PublisherLedgerEntryEntity> PublisherLedgerEntries { get; }
    IQueryable<PublisherWithdrawalEntity> PublisherWithdrawals { get; }



    Task AddWalletAsync(WalletEntity wallet, CancellationToken ct);
    Task AddReservationAsync(ReservationEntity reservation, CancellationToken ct);
    Task AddWalletTransactionAsync(WalletTransactionEntity tx, CancellationToken ct);
    Task AddTopUpAsync(TopUp topUp, CancellationToken ct);
    Task AddYooKassaWebhookInboxMessageAsync(YooKassaWebhookInboxMessage message, CancellationToken ct);
    Task AddPublisherWalletAsync(PublisherWalletEntity wallet, CancellationToken ct);
    Task AddPublisherLedgerEntryAsync(PublisherLedgerEntryEntity entry, CancellationToken ct);
    Task AddPublisherWithdrawalAsync(PublisherWithdrawalEntity withdrawal, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
