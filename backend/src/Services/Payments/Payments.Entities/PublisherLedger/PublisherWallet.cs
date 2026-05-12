using Payments.Entities.Common;

namespace Payments.Entities.PublisherLedger;

/// <summary>
/// Кошелек паблишера.
/// Available хранит начисления за завершенные сделки.
/// </summary>
public sealed class PublisherWallet : Entity
{
    private PublisherWallet() { }

    public Guid WalletId { get; private set; } = Guid.NewGuid();
    public Guid PublisherUserId { get; private set; }

    public string Currency { get; private set; } = "RUB";

    public decimal Available { get; private set; }
    public decimal PaidOut { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Создает пустой кошелек паблишера.
    /// </summary>
    public static PublisherWallet Create(Guid publisherUserId, string currency, DateTimeOffset nowUtc)
    {
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new PublisherWallet
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            PublisherUserId = publisherUserId,
            Currency = currency.Trim().ToUpperInvariant(),
            Available = 0m,
            PaidOut = 0m,
            UpdatedAt = nowUtc
        };
    }

    public void AddAvailable(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");

        // Начисление увеличивает общий Available.
        Available += amount;
        UpdatedAt = nowUtc;
    }

    public void MarkPaidOut(decimal amount, DateTimeOffset nowUtc)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");
        if (Available < amount) throw new InvalidOperationException("InsufficientAvailableAmount");

        // Вывод переносит сумму из Available в PaidOut.
        Available -= amount;
        PaidOut += amount;
        UpdatedAt = nowUtc;
    }
}
