using Payments.Entities.Common;

namespace Payments.Entities.TopUps;

public sealed class TopUp : Entity
{
    private TopUp() { }

    public Guid TopUpId { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public string YooKassaPaymentId { get; private set; } = default!;
    public string? ConfirmationUrl { get; private set; }

    public TopUpStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static TopUp Create(Guid userId, decimal amount, string currency, DateTimeOffset nowUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId required");
        if (amount <= 0) throw new ArgumentException("Amount must be > 0");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new TopUp
        {
            Id = Guid.NewGuid(),
            TopUpId = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Currency = currency,
            Status = TopUpStatus.Created,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    public void SetYooKassa(string yooId, string? confirmationUrl, DateTimeOffset nowUtc)
    {
        YooKassaPaymentId = yooId;
        ConfirmationUrl = confirmationUrl;
        UpdatedAt = nowUtc;
    }

    public void MarkSucceeded(DateTimeOffset nowUtc)
    {
        Status = TopUpStatus.Succeeded;
        UpdatedAt = nowUtc;
    }

    public void MarkCanceled(DateTimeOffset nowUtc)
    {
        Status = TopUpStatus.Canceled;
        UpdatedAt = nowUtc;
    }
}
