using Marketplace.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Payments.Entities.Payments;
using Payments.Entities.TopUps;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using ReservationEntity = Payments.Entities.Wallet.Reservation;
using WalletTransactionEntity = Payments.Entities.Wallet.WalletTransaction;
using Payments.UseCases.Abstractions.Persistence;

namespace Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContext : DbContext, IPaymentsDbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

    public DbSet<Payment> PaymentsSet => Set<Payment>();
    public IQueryable<Payment> Payments => PaymentsSet.AsQueryable();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<WalletEntity> WalletsSet => Set<WalletEntity>();
    public IQueryable<WalletEntity> Wallets => WalletsSet.AsQueryable();
    public DbSet<ReservationEntity> ReservationsSet => Set<ReservationEntity>();
    public IQueryable<ReservationEntity> Reservations => ReservationsSet.AsQueryable();
    public DbSet<WalletTransactionEntity> WalletTransactionsSet => Set<WalletTransactionEntity>();
    public IQueryable<WalletTransactionEntity> WalletTransactions => WalletTransactionsSet.AsQueryable();
    public DbSet<TopUp> TopUpsSet => Set<TopUp>();
    public IQueryable<TopUp> TopUps => TopUpsSet.AsQueryable();

    public Task AddPaymentAsync(Payment payment, CancellationToken ct) => PaymentsSet.AddAsync(payment, ct).AsTask();

    public Task<Payment?> FindByPaymentIdAsync(Guid paymentId, CancellationToken ct)
        => PaymentsSet.FirstOrDefaultAsync(x => x.PaymentId == paymentId, ct);

    public Task<Payment?> FindByYooKassaIdAsync(string yooId, CancellationToken ct)
        => PaymentsSet.FirstOrDefaultAsync(x => x.YooKassaPaymentId == yooId, ct);

    public Task AddWalletAsync(WalletEntity wallet, CancellationToken ct) => WalletsSet.AddAsync(wallet, ct).AsTask();
    public Task AddReservationAsync(ReservationEntity reservation, CancellationToken ct) => ReservationsSet.AddAsync(reservation, ct).AsTask();
    public Task AddWalletTransactionAsync(WalletTransactionEntity tx, CancellationToken ct) => WalletTransactionsSet.AddAsync(tx, ct).AsTask();
    public Task AddTopUpAsync(TopUp topUp, CancellationToken ct) => TopUpsSet.AddAsync(topUp, ct).AsTask();

    Task<int> IPaymentsDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
}
