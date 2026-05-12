using Payments.Entities.Common;

namespace Payments.Entities.PublisherLedger;

/// <summary>
/// Заявка паблишера на вывод средств.
/// </summary>
public sealed class PublisherWithdrawal : Entity
{
    private PublisherWithdrawal() { }

    public Guid WithdrawalId { get; private set; } = Guid.NewGuid();
    public Guid PublisherUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";
    public string DestinationCardMask { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Создает запись вывода с маской карты.
    /// </summary>
    public static PublisherWithdrawal Create(
        Guid publisherUserId,
        decimal amount,
        string currency,
        string destinationCardMask,
        DateTimeOffset nowUtc)
    {
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (amount <= 0) throw new ArgumentException("Amount must be > 0");
        if (!string.Equals(currency, "RUB", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only RUB is supported in MVP");
        if (string.IsNullOrWhiteSpace(destinationCardMask))
            throw new ArgumentException("DestinationCardMask required");

        return new PublisherWithdrawal
        {
            Id = Guid.NewGuid(),
            WithdrawalId = Guid.NewGuid(),
            PublisherUserId = publisherUserId,
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
            Currency = currency.Trim().ToUpperInvariant(),
            DestinationCardMask = destinationCardMask.Trim(),
            CreatedAt = nowUtc
        };
    }
}
