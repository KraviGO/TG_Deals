using Payments.Entities.Common;

namespace Payments.Entities.PublisherLedger;

/// <summary>
/// Начисление паблишеру за завершенную сделку.
/// </summary>
public sealed class PublisherLedgerEntry : Entity
{
    private PublisherLedgerEntry() { }

    public Guid EntryId { get; private set; } = Guid.NewGuid();

    public Guid DealId { get; private set; }
    public Guid PublisherUserId { get; private set; }

    public decimal GrossAmount { get; private set; }
    public decimal PlatformFeeAmount { get; private set; }
    public decimal PublisherAmount { get; private set; }

    public string Currency { get; private set; } = "RUB";

    public PublisherLedgerEntryStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? AvailableAt { get; private set; }

    /// <summary>
    /// Создает начисление с расчетом gross, fee и publisher amount.
    /// </summary>
    public static PublisherLedgerEntry CreateAccrual(
        Guid dealId,
        Guid publisherUserId,
        decimal grossAmount,
        decimal platformFeeAmount,
        decimal publisherAmount,
        string currency,
        DateTimeOffset nowUtc)
    {
        if (dealId == Guid.Empty) throw new ArgumentException("DealId required");
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (grossAmount <= 0) throw new ArgumentException("GrossAmount must be > 0");
        if (platformFeeAmount < 0) throw new ArgumentException("PlatformFeeAmount must be >= 0");
        if (publisherAmount < 0) throw new ArgumentException("PublisherAmount must be >= 0");
        if (!string.Equals(currency, "RUB", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only RUB is supported in MVP");

        return new PublisherLedgerEntry
        {
            Id = Guid.NewGuid(),
            EntryId = Guid.NewGuid(),
            DealId = dealId,
            PublisherUserId = publisherUserId,
            GrossAmount = grossAmount,
            PlatformFeeAmount = platformFeeAmount,
            PublisherAmount = publisherAmount,
            Currency = currency.Trim().ToUpperInvariant(),
            Status = PublisherLedgerEntryStatus.Accrued,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc,
            AvailableAt = nowUtc
        };
    }
}
