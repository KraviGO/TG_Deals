using Deals.Entities.Common;

namespace Deals.Entities.Deals;

public sealed class Deal : Entity
{
    private Deal() { } // EF

    public Guid DealId { get; private set; } = Guid.NewGuid();

    public Guid ChannelId { get; private set; }
    public Guid PublisherUserId { get; private set; }
    public Guid AdvertiserUserId { get; private set; }

    public string PostText { get; private set; } = default!;
    public DateTimeOffset DesiredPublishAtUtc { get; private set; }

    public Guid? PaymentId { get; private set; }
    public string? PaymentState { get; private set; }

    public DealStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Deal Create(
        Guid channelId,
        Guid publisherUserId,
        Guid advertiserUserId,
        string postText,
        DateTimeOffset desiredPublishAtUtc,
        DealStatus initialStatus,
        DateTimeOffset nowUtc)
    {
        if (channelId == Guid.Empty) throw new ArgumentException("ChannelId required");
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (advertiserUserId == Guid.Empty) throw new ArgumentException("AdvertiserUserId required");
        if (string.IsNullOrWhiteSpace(postText)) throw new ArgumentException("PostText required");

        return new Deal
        {
            Id = Guid.NewGuid(),
            DealId = Guid.NewGuid(),
            ChannelId = channelId,
            PublisherUserId = publisherUserId,
            AdvertiserUserId = advertiserUserId,
            PostText = postText.Trim(),
            DesiredPublishAtUtc = desiredPublishAtUtc,
            Status = initialStatus,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    public void AttachPayment(Guid paymentId, DateTimeOffset nowUtc)
    {
        PaymentId = paymentId;
        PaymentState = "AwaitingPayment";
        UpdatedAt = nowUtc;
    }

    public void SetPaymentState(string state, DateTimeOffset nowUtc)
    {
        PaymentState = state;
        UpdatedAt = nowUtc;
    }

    public void PublisherAccept(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.PendingPublisherDecision)
            throw new InvalidOperationException("Deal is not awaiting publisher decision.");
        Status = DealStatus.Accepted;
        UpdatedAt = nowUtc;
    }

    public void PublisherReject(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.PendingPublisherDecision)
            throw new InvalidOperationException("Deal is not awaiting publisher decision.");
        Status = DealStatus.Rejected;
        UpdatedAt = nowUtc;
    }

    public void CancelByAdvertiser(DateTimeOffset nowUtc)
    {
        if (Status == DealStatus.Rejected || Status == DealStatus.CancelledByAdvertiser)
            throw new InvalidOperationException("Deal cannot be cancelled.");
        if (Status == DealStatus.Accepted)
            throw new InvalidOperationException("Cannot cancel after accepted in MVP.");
        Status = DealStatus.CancelledByAdvertiser;
        UpdatedAt = nowUtc;
    }
}
