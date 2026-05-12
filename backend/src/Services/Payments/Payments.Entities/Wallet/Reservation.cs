using Payments.Entities.Common;

namespace Payments.Entities.Wallet;

/// <summary>
/// Резерв денег рекламодателя под конкретную сделку.
/// </summary>
public sealed class Reservation : Entity
{
    private Reservation() { }

    public Guid ReservationId { get; private set; } = Guid.NewGuid();

    public Guid DealId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid PublisherUserId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public ReservationStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Создает резерв в статусе Reserved.
    /// </summary>
    public static Reservation Create(Guid dealId, Guid userId, Guid publisherUserId, decimal amount, string currency, DateTimeOffset nowUtc)
    {
        if (dealId == Guid.Empty) throw new ArgumentException("DealId required");
        if (userId == Guid.Empty) throw new ArgumentException("UserId required");
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (amount <= 0) throw new ArgumentException("Amount must be > 0");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new Reservation
        {
            Id = Guid.NewGuid(),
            ReservationId = Guid.NewGuid(),
            DealId = dealId,
            UserId = userId,
            PublisherUserId = publisherUserId,
            Amount = amount,
            Currency = currency,
            Status = ReservationStatus.Reserved,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    /// <summary>
    /// Помечает резерв возвращенным рекламодателю.
    /// </summary>
    public void MarkReleased(DateTimeOffset nowUtc)
    {
        if (Status != ReservationStatus.Reserved)
            throw new InvalidOperationException("Reservation is not Reserved.");
        Status = ReservationStatus.Released;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Помечает резерв списанным в пользу паблишера.
    /// </summary>
    public void MarkCaptured(DateTimeOffset nowUtc)
    {
        if (Status != ReservationStatus.Reserved)
            throw new InvalidOperationException("Reservation is not Reserved.");
        Status = ReservationStatus.Captured;
        UpdatedAt = nowUtc;
    }
}
