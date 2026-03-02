using Payments.Entities.Payments;
using Payments.Entities.TopUps;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using ReservationEntity = Payments.Entities.Wallet.Reservation;
using WalletTransactionEntity = Payments.Entities.Wallet.WalletTransaction;

namespace Payments.UseCases.Abstractions.Persistence;

public interface IPaymentsDbContext
{
    IQueryable<Payment> Payments { get; }
    IQueryable<WalletEntity> Wallets { get; }
    IQueryable<ReservationEntity> Reservations { get; }
    IQueryable<WalletTransactionEntity> WalletTransactions { get; }
    IQueryable<TopUp> TopUps { get; }

    Task AddPaymentAsync(Payment payment, CancellationToken ct);
    Task<Payment?> FindByPaymentIdAsync(Guid paymentId, CancellationToken ct);
    Task<Payment?> FindByYooKassaIdAsync(string yooId, CancellationToken ct);

    Task AddWalletAsync(WalletEntity wallet, CancellationToken ct);
    Task AddReservationAsync(ReservationEntity reservation, CancellationToken ct);
    Task AddWalletTransactionAsync(WalletTransactionEntity tx, CancellationToken ct);
    Task AddTopUpAsync(TopUp topUp, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
