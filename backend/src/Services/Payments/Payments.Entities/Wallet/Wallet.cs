using Payments.Entities.Common;

namespace Payments.Entities.Wallet;

public sealed class Wallet : Entity
{
    private Wallet() { }

    public Guid WalletId { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public decimal Available { get; private set; }
    public decimal Reserved { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Wallet Create(Guid userId, string currency, DateTimeOffset nowUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId required");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new Wallet
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            UserId = userId,
            Currency = currency,
            Available = 0m,
            Reserved = 0m,
            UpdatedAt = nowUtc
        };
    }

    public void Credit(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");
        Available += amount;
        UpdatedAt = nowUtc;
    }

    public void ReserveFunds(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");
        if (Available < amount) throw new InvalidOperationException("InsufficientFunds");

        Available -= amount;
        Reserved += amount;
        UpdatedAt = nowUtc;
    }

    public void ReleaseFunds(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");
        if (Reserved < amount) throw new InvalidOperationException("InvalidReservedAmount");

        Reserved -= amount;
        Available += amount;
        UpdatedAt = nowUtc;
    }

    public void CaptureFunds(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");
        if (Reserved < amount) throw new InvalidOperationException("InvalidReservedAmount");

        Reserved -= amount;
        UpdatedAt = nowUtc;
    }
}
