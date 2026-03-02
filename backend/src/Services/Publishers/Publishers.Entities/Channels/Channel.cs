using Publishers.Entities.Common;

namespace Publishers.Entities.Channels;

public sealed class Channel : Entity
{
    private Channel() { }

    public ChannelId ChannelId { get; private set; }
    public Guid PublisherUserId { get; private set; }

    public string TelegramChannelId { get; private set; } = default!;
    public string Title { get; private set; } = default!;

    public IntakeMode IntakeMode { get; private set; } = IntakeMode.ActiveManual;
    public OwnershipStatus OwnershipStatus { get; private set; } = OwnershipStatus.PendingVerification;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public string? VerificationCode { get; private set; }
    public DateTimeOffset? VerificationExpiresAt { get; private set; }

    public static Channel Create(Guid publisherUserId, string telegramChannelId, ChannelTitle title, DateTimeOffset now)
    {
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId is required.");
        if (string.IsNullOrWhiteSpace(telegramChannelId)) throw new ArgumentException("TelegramChannelId is required.");

        var ch = new Channel
        {
            Id = Guid.NewGuid(),
            ChannelId = ChannelId.New(),
            PublisherUserId = publisherUserId,
            TelegramChannelId = telegramChannelId.Trim(),
            Title = title.Value,
            IntakeMode = IntakeMode.ActiveManual,
            OwnershipStatus = OwnershipStatus.PendingVerification,
            CreatedAt = now,
            UpdatedAt = now
        };

        return ch;
    }

    public void Update(string telegramChannelId, ChannelTitle title, DateTimeOffset now)
    {
        TelegramChannelId = telegramChannelId.Trim();
        Title = title.Value;
        UpdatedAt = now;
    }

    public void SetIntakeMode(IntakeMode mode, DateTimeOffset now)
    {
        IntakeMode = mode;
        UpdatedAt = now;
    }

    public string StartVerification(string code, DateTimeOffset expiresAt, DateTimeOffset now)
    {
        if (OwnershipStatus == OwnershipStatus.Verified)
            throw new InvalidOperationException("Channel already verified.");

        VerificationCode = code;
        VerificationExpiresAt = expiresAt;
        UpdatedAt = now;

        return $"Поставь этот код в описании канала или закрепе: {code}";
    }

    public void ConfirmVerification(DateTimeOffset now)
    {
        if (VerificationCode is null || VerificationExpiresAt is null)
            throw new InvalidOperationException("Verification not started.");

        if (now > VerificationExpiresAt.Value)
            throw new InvalidOperationException("Verification expired.");

        OwnershipStatus = OwnershipStatus.Verified;
        VerificationCode = null;
        VerificationExpiresAt = null;
        UpdatedAt = now;
    }
}
