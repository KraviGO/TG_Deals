using Deals.Entities.Common;

namespace Deals.Entities.Deals;

/// <summary>
/// Агрегат сделки между рекламодателем и паблишером.
/// </summary>
public sealed class Deal : Entity
{
    private Deal() { }

    public Guid DealId { get; private set; } = Guid.NewGuid();
    public Guid ChannelId { get; private set; }
    public Guid PublisherUserId { get; private set; }
    public Guid AdvertiserUserId { get; private set; }
    public string PostText { get; private set; } = default!;
    public DateTimeOffset DesiredPublishAtUtc { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";
    public FundingStatus FundingStatus { get; private set; } = FundingStatus.None;
    public Guid? ReservationId { get; private set; }
    public string? PostUrl { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public string? PublisherComment { get; private set; }
    public DealStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Создает сделку в начальном статусе.
    /// </summary>
    public static Deal Create(
        Guid channelId,
        Guid publisherUserId,
        Guid advertiserUserId,
        string postText,
        DateTimeOffset desiredPublishAtUtc,
        decimal amount,
        string currency,
        DealStatus initialStatus,
        DateTimeOffset nowUtc)
    {
        if (channelId == Guid.Empty) throw new ArgumentException("ChannelId required");
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId required");
        if (advertiserUserId == Guid.Empty) throw new ArgumentException("AdvertiserUserId required");
        if (string.IsNullOrWhiteSpace(postText)) throw new ArgumentException("PostText required");
        if (amount <= 0) throw new ArgumentException("Amount must be > 0");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required");

        return new Deal
        {
            Id = Guid.NewGuid(),
            DealId = Guid.NewGuid(),
            ChannelId = channelId,
            PublisherUserId = publisherUserId,
            AdvertiserUserId = advertiserUserId,
            PostText = postText.Trim(),
            DesiredPublishAtUtc = desiredPublishAtUtc,
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
            FundingStatus = FundingStatus.None,
            Status = initialStatus,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    /// <summary>
    /// Переводит заявку в Accepted.
    /// </summary>
    public void PublisherAccept(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.PendingPublisherDecision)
            throw new InvalidOperationException("Deal is not awaiting publisher decision.");

        Status = DealStatus.Accepted;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Переводит заявку в Rejected.
    /// </summary>
    public void PublisherReject(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.PendingPublisherDecision)
            throw new InvalidOperationException("Deal is not awaiting publisher decision.");

        Status = DealStatus.Rejected;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Фиксирует резерв в Payments.
    /// </summary>
    public void MarkFundingReserved(Guid reservationId, DateTimeOffset nowUtc)
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("ReservationId required.");

        if (FundingStatus == FundingStatus.Reserved && ReservationId == reservationId)
            return;

        if (FundingStatus != FundingStatus.None)
            throw new InvalidOperationException("Funding already finalized for this deal.");

        ReservationId = reservationId;
        FundingStatus = FundingStatus.Reserved;
        Status = DealStatus.FundingReserved;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Фиксирует возврат резерва рекламодателю.
    /// </summary>
    public void MarkFundingReleased(DateTimeOffset nowUtc)
    {
        if (FundingStatus == FundingStatus.Released)
            return;

        if (FundingStatus != FundingStatus.Reserved)
            throw new InvalidOperationException("Funding is not reserved.");

        FundingStatus = FundingStatus.Released;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Фиксирует опубликованный пост.
    /// </summary>
    public void PublisherConfirmPublished(string postUrl, DateTimeOffset publishedAtUtc, string? publisherComment, DateTimeOffset nowUtc)
    {
        if (FundingStatus != FundingStatus.Reserved)
            throw new InvalidOperationException("Cannot confirm publication without reserved funding.");

        if (Status != DealStatus.FundingReserved && Status != DealStatus.ReadyToPublish && Status != DealStatus.PublishedPendingConfirm)
            throw new InvalidOperationException("Deal cannot be confirmed in current status.");

        if (string.IsNullOrWhiteSpace(postUrl))
            throw new ArgumentException("PostUrl required.");

        PostUrl = postUrl.Trim();
        PublishedAtUtc = publishedAtUtc;
        PublisherComment = string.IsNullOrWhiteSpace(publisherComment) ? null : publisherComment.Trim();

        Status = DealStatus.PublishedPendingConfirm;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Завершает сделку после Capture в Payments.
    /// </summary>
    public void AdvertiserConfirmAndComplete(DateTimeOffset nowUtc)
    {
        if (FundingStatus != FundingStatus.Reserved)
            throw new InvalidOperationException("Cannot capture deal without reserved funding.");

        if (Status != DealStatus.PublishedPendingConfirm)
            throw new InvalidOperationException("Deal is not awaiting advertiser confirmation.");

        FundingStatus = FundingStatus.Captured;
        Status = DealStatus.Completed;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Отменяет сделку рекламодателем.
    /// </summary>
    public void CancelByAdvertiser(DateTimeOffset nowUtc)
    {
        if (Status is DealStatus.Rejected or DealStatus.CanceledByAdvertiser or DealStatus.Completed)
            throw new InvalidOperationException("Deal cannot be cancelled.");

        Status = DealStatus.CanceledByAdvertiser;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Переводит сделку в спор.
    /// </summary>
    public void MarkDisputed(DateTimeOffset nowUtc)
    {
        if (Status is DealStatus.CanceledByAdvertiser or DealStatus.Rejected or DealStatus.Resolved)
            throw new InvalidOperationException("Deal cannot be disputed in current state.");

        Status = DealStatus.Disputed;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Решает спор списанием резерва паблишеру.
    /// </summary>
    public void ResolveAfterCapture(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.Disputed)
            throw new InvalidOperationException("Deal is not disputed.");

        if (FundingStatus == FundingStatus.Reserved)
            FundingStatus = FundingStatus.Captured;
        else if (FundingStatus != FundingStatus.Captured)
            throw new InvalidOperationException("Deal funding is not capturable.");

        Status = DealStatus.Resolved;
        UpdatedAt = nowUtc;
    }

    /// <summary>
    /// Решает спор возвратом резерва рекламодателю.
    /// </summary>
    public void ResolveAfterRelease(DateTimeOffset nowUtc)
    {
        if (Status != DealStatus.Disputed)
            throw new InvalidOperationException("Deal is not disputed.");

        if (FundingStatus == FundingStatus.Reserved)
            FundingStatus = FundingStatus.Released;
        else if (FundingStatus != FundingStatus.Released)
            throw new InvalidOperationException("Deal funding is not releasable.");

        Status = DealStatus.Resolved;
        UpdatedAt = nowUtc;
    }
}
