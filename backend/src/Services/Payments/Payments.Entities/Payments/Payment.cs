using Payments.Entities.Common;

namespace Payments.Entities.Payments;

public sealed class Payment : Entity
{
    private Payment() { }

    public Guid PaymentId { get; private set; } = Guid.NewGuid();

    public Guid DealId { get; private set; }
    public Guid AdvertiserUserId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public string YooKassaPaymentId { get; private set; } = default!;
    public string? ConfirmationUrl { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Payment Create(Guid dealId, Guid advertiserUserId, decimal amount, string currency, DateTimeOffset nowUtc)
    {
        if (dealId == Guid.Empty) throw new ArgumentException("DealId required");
        if (advertiserUserId == Guid.Empty) throw new ArgumentException("AdvertiserUserId required");
        if (amount <= 0) throw new ArgumentException("Amount must be > 0");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new Payment
        {
            Id = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            DealId = dealId,
            AdvertiserUserId = advertiserUserId,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Created,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    public void SetYooKassaInfo(string yooPaymentId, string? confirmationUrl, DateTimeOffset nowUtc)
    {
        YooKassaPaymentId = yooPaymentId;
        ConfirmationUrl = confirmationUrl;
        UpdatedAt = nowUtc;
    }

    public void MarkAuthorized(DateTimeOffset nowUtc)
    {
        Status = PaymentStatus.Authorized;
        UpdatedAt = nowUtc;
    }

    public void MarkCaptured(DateTimeOffset nowUtc)
    {
        Status = PaymentStatus.Captured;
        UpdatedAt = nowUtc;
    }

    public void MarkCanceled(DateTimeOffset nowUtc)
    {
        Status = PaymentStatus.Canceled;
        UpdatedAt = nowUtc;
    }
}
