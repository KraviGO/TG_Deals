using Publishers.Entities.Common;

namespace Publishers.Entities.Channels;

/// <summary>
/// Агрегат Telegram-канала паблишера.
/// </summary>
public sealed class Channel : Entity
{
    public const string DefaultTopic = "General";
    public const string DefaultLanguage = "ru";
    public const decimal DefaultPricePerPostRub = 1000m;

    private Channel() { }

    public ChannelId ChannelId { get; private set; }
    public Guid PublisherUserId { get; private set; }
    public string TelegramChannelId { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Topic { get; private set; } = default!;
    public string Language { get; private set; } = default!;
    public decimal PricePerPostRub { get; private set; }
    public IntakeMode IntakeMode { get; private set; } = IntakeMode.ActiveManual;
    public OwnershipStatus OwnershipStatus { get; private set; } = OwnershipStatus.PendingVerification;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Создает канал со статусом PendingVerification.
    /// </summary>
    public static Channel Create(
        Guid publisherUserId,
        string telegramChannelId,
        ChannelTitle title,
        string? topic,
        string? language,
        decimal? pricePerPostRub,
        DateTimeOffset now)
    {
        if (publisherUserId == Guid.Empty) throw new ArgumentException("PublisherUserId is required.");
        var normalizedTelegramChannelId = NormalizeTelegramChannelId(telegramChannelId);

        var resolvedTopic = NormalizeTopic(topic);
        var resolvedLanguage = NormalizeLanguage(language);
        var resolvedPrice = ValidatePrice(pricePerPostRub ?? DefaultPricePerPostRub);

        var ch = new Channel
        {
            Id = Guid.NewGuid(),
            ChannelId = ChannelId.New(),
            PublisherUserId = publisherUserId,
            TelegramChannelId = normalizedTelegramChannelId,
            Title = title.Value,
            Topic = resolvedTopic,
            Language = resolvedLanguage,
            PricePerPostRub = resolvedPrice,
            IntakeMode = IntakeMode.ActiveManual,
            OwnershipStatus = OwnershipStatus.PendingVerification,
            CreatedAt = now,
            UpdatedAt = now
        };

        return ch;
    }

    /// <summary>
    /// Обновляет профиль канала для каталога.
    /// </summary>
    public void Update(
        string telegramChannelId,
        ChannelTitle title,
        string? topic,
        string? language,
        decimal? pricePerPostRub,
        DateTimeOffset now)
    {
        EnsureNotBanned();
        TelegramChannelId = NormalizeTelegramChannelId(telegramChannelId);
        Title = title.Value;
        Topic = NormalizeTopic(topic);
        Language = NormalizeLanguage(language);
        PricePerPostRub = ValidatePrice(pricePerPostRub ?? PricePerPostRub);
        UpdatedAt = now;
    }

    public void SetIntakeMode(IntakeMode mode, DateTimeOffset now)
    {
        EnsureNotBanned();
        IntakeMode = mode;
        UpdatedAt = now;
    }

    /// <summary>
    /// Обновляет профиль каталога без смены TelegramChannelId и Title.
    /// </summary>
    public void SetCatalogProfile(string? topic, string? language, decimal? pricePerPostRub, DateTimeOffset now)
    {
        EnsureNotBanned();
        Topic = NormalizeTopic(topic);
        Language = NormalizeLanguage(language);
        PricePerPostRub = ValidatePrice(pricePerPostRub ?? PricePerPostRub);
        UpdatedAt = now;
    }

    /// <summary>
    /// Переводит канал в Verified после проверки прав бота.
    /// </summary>
    public bool VerifyOwnership(DateTimeOffset now)
    {
        EnsureNotBanned();
        if (OwnershipStatus == OwnershipStatus.Verified)
            return false;

        OwnershipStatus = OwnershipStatus.Verified;
        UpdatedAt = now;

        return true;
    }

    /// <summary>
    /// Банит канал без обратного перехода.
    /// </summary>
    public void Ban(DateTimeOffset now)
    {
        OwnershipStatus = OwnershipStatus.Banned;
        UpdatedAt = now;
    }

    /// <summary>
    /// Бан канала является финальным.
    /// </summary>
    public void UnbanToVerified(DateTimeOffset now)
    {
        _ = now;
        throw new InvalidOperationException("ChannelBanIsFinal");
    }

    private void EnsureNotBanned()
    {
        if (OwnershipStatus == OwnershipStatus.Banned)
            throw new InvalidOperationException("ChannelBanIsFinal");
    }

    private static string NormalizeTopic(string? topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return DefaultTopic;

        return topic.Trim();
    }

    private static string NormalizeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return DefaultLanguage;

        return language.Trim().ToLowerInvariant();
    }

    private static decimal ValidatePrice(decimal pricePerPostRub)
    {
        if (pricePerPostRub <= 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerPostRub), "Price must be positive.");

        return decimal.Round(pricePerPostRub, 2, MidpointRounding.AwayFromZero);
    }

    public static string NormalizeTelegramChannelId(string telegramChannelId)
    {
        if (string.IsNullOrWhiteSpace(telegramChannelId))
            throw new ArgumentException("TelegramChannelId is required.");

        var value = telegramChannelId.Trim();
        if (value.StartsWith("https://t.me/", StringComparison.OrdinalIgnoreCase))
            value = value["https://t.me/".Length..].Split('/')[0];

        if (value.StartsWith("@", StringComparison.Ordinal))
            value = value[1..];

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("TelegramChannelId is required.");

        return value.Trim();
    }
}
