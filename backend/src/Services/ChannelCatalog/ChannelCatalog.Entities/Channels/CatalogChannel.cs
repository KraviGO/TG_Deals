using ChannelCatalog.Entities.Common;

namespace ChannelCatalog.Entities.Channels;

public sealed class CatalogChannel : Entity
{
    private CatalogChannel() { }

    public Guid ChannelId { get; private set; }
    public Guid PublisherUserId { get; private set; }

    public string TelegramChannelId { get; private set; } = default!;
    public string Title { get; private set; } = default!;

    public string IntakeMode { get; private set; } = default!;
    public string OwnershipStatus { get; private set; } = default!;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static CatalogChannel CreateFromRegistered(
        Guid channelId,
        Guid publisherUserId,
        string telegramChannelId,
        string title,
        string intakeMode,
        string ownershipStatus,
        DateTimeOffset nowUtc)
    {
        return new CatalogChannel
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            PublisherUserId = publisherUserId,
            TelegramChannelId = telegramChannelId,
            Title = title,
            IntakeMode = intakeMode,
            OwnershipStatus = ownershipStatus,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    public void ApplyRegistered(
        Guid publisherUserId,
        string telegramChannelId,
        string title,
        string intakeMode,
        string ownershipStatus,
        DateTimeOffset nowUtc)
    {
        PublisherUserId = publisherUserId;
        TelegramChannelId = telegramChannelId;
        Title = title;
        IntakeMode = intakeMode;
        OwnershipStatus = ownershipStatus;
        UpdatedAt = nowUtc;
    }

    public void MarkVerified(DateTimeOffset nowUtc)
    {
        OwnershipStatus = "Verified";
        UpdatedAt = nowUtc;
    }
}
