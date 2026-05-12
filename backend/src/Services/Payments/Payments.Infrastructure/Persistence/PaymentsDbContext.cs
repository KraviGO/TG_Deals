using Microsoft.EntityFrameworkCore;

using Payments.Entities.PublisherLedger;
using Payments.Entities.TopUps;
using Payments.Entities.Wallet;
using Payments.Entities.Webhooks;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using ReservationEntity = Payments.Entities.Wallet.Reservation;
using WalletTransactionEntity = Payments.Entities.Wallet.WalletTransaction;
using PublisherWalletEntity = Payments.Entities.PublisherLedger.PublisherWallet;
using PublisherLedgerEntryEntity = Payments.Entities.PublisherLedger.PublisherLedgerEntry;
using PublisherWithdrawalEntity = Payments.Entities.PublisherLedger.PublisherWithdrawal;
using Payments.UseCases.Abstractions.Persistence;

namespace Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContext : DbContext, IPaymentsDbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

    public DbSet<WalletEntity> WalletsSet => Set<WalletEntity>();
    public IQueryable<WalletEntity> Wallets => WalletsSet.AsQueryable();
    public DbSet<ReservationEntity> ReservationsSet => Set<ReservationEntity>();
    public IQueryable<ReservationEntity> Reservations => ReservationsSet.AsQueryable();
    public DbSet<WalletTransactionEntity> WalletTransactionsSet => Set<WalletTransactionEntity>();
    public IQueryable<WalletTransactionEntity> WalletTransactions => WalletTransactionsSet.AsQueryable();
    public DbSet<TopUp> TopUpsSet => Set<TopUp>();
    public IQueryable<TopUp> TopUps => TopUpsSet.AsQueryable();
    public DbSet<YooKassaWebhookInboxMessage> YooKassaWebhookInboxMessagesSet => Set<YooKassaWebhookInboxMessage>();
    public IQueryable<YooKassaWebhookInboxMessage> YooKassaWebhookInboxMessages => YooKassaWebhookInboxMessagesSet.AsQueryable();
    public DbSet<PublisherWalletEntity> PublisherWalletsSet => Set<PublisherWalletEntity>();
    public IQueryable<PublisherWalletEntity> PublisherWallets => PublisherWalletsSet.AsQueryable();
    public DbSet<PublisherLedgerEntryEntity> PublisherLedgerEntriesSet => Set<PublisherLedgerEntryEntity>();
    public IQueryable<PublisherLedgerEntryEntity> PublisherLedgerEntries => PublisherLedgerEntriesSet.AsQueryable();
    public DbSet<PublisherWithdrawalEntity> PublisherWithdrawalsSet => Set<PublisherWithdrawalEntity>();
    public IQueryable<PublisherWithdrawalEntity> PublisherWithdrawals => PublisherWithdrawalsSet.AsQueryable();



    public Task AddWalletAsync(WalletEntity wallet, CancellationToken ct) => WalletsSet.AddAsync(wallet, ct).AsTask();
    public Task AddReservationAsync(ReservationEntity reservation, CancellationToken ct) => ReservationsSet.AddAsync(reservation, ct).AsTask();
    public Task AddWalletTransactionAsync(WalletTransactionEntity tx, CancellationToken ct) => WalletTransactionsSet.AddAsync(tx, ct).AsTask();
    public Task AddTopUpAsync(TopUp topUp, CancellationToken ct) => TopUpsSet.AddAsync(topUp, ct).AsTask();
    public Task AddYooKassaWebhookInboxMessageAsync(YooKassaWebhookInboxMessage message, CancellationToken ct) => YooKassaWebhookInboxMessagesSet.AddAsync(message, ct).AsTask();
    public Task AddPublisherWalletAsync(PublisherWalletEntity wallet, CancellationToken ct) => PublisherWalletsSet.AddAsync(wallet, ct).AsTask();
    public Task AddPublisherLedgerEntryAsync(PublisherLedgerEntryEntity entry, CancellationToken ct) => PublisherLedgerEntriesSet.AddAsync(entry, ct).AsTask();
    public Task AddPublisherWithdrawalAsync(PublisherWithdrawalEntity withdrawal, CancellationToken ct) => PublisherWithdrawalsSet.AddAsync(withdrawal, ct).AsTask();

    Task<int> IPaymentsDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
}
