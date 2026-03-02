using Payments.Entities.Common;

namespace Payments.Entities.Wallet;

public sealed class WalletTransaction : Entity
{
    private WalletTransaction() { }

    public Guid TxId { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }
    public WalletTransactionType Type { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public Guid? DealId { get; private set; }
    public Guid? TopUpId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static WalletTransaction CreateTopUp(Guid userId, Guid topUpId, decimal amount, string currency, DateTimeOffset nowUtc)
        => new()
        {
            Id = Guid.NewGuid(),
            TxId = Guid.NewGuid(),
            UserId = userId,
            Type = WalletTransactionType.TopUpSucceeded,
            Amount = amount,
            Currency = currency,
            TopUpId = topUpId,
            CreatedAt = nowUtc
        };

    public static WalletTransaction CreateReserve(Guid userId, Guid dealId, decimal amount, string currency, DateTimeOffset nowUtc)
        => new()
        {
            Id = Guid.NewGuid(),
            TxId = Guid.NewGuid(),
            UserId = userId,
            Type = WalletTransactionType.ReserveCreated,
            Amount = amount,
            Currency = currency,
            DealId = dealId,
            CreatedAt = nowUtc
        };

    public static WalletTransaction CreateRelease(Guid userId, Guid dealId, decimal amount, string currency, DateTimeOffset nowUtc)
        => new()
        {
            Id = Guid.NewGuid(),
            TxId = Guid.NewGuid(),
            UserId = userId,
            Type = WalletTransactionType.ReserveReleased,
            Amount = amount,
            Currency = currency,
            DealId = dealId,
            CreatedAt = nowUtc
        };

    public static WalletTransaction CreateCapture(Guid userId, Guid dealId, decimal amount, string currency, DateTimeOffset nowUtc)
        => new()
        {
            Id = Guid.NewGuid(),
            TxId = Guid.NewGuid(),
            UserId = userId,
            Type = WalletTransactionType.ReserveCaptured,
            Amount = amount,
            Currency = currency,
            DealId = dealId,
            CreatedAt = nowUtc
        };
}
