namespace Marketplace.Contracts.Publishers;

/// <summary>
/// Событие успешного подтверждения владения Telegram-каналом.
/// Подписчики переводят канал в следующие этапы своего процесса.
/// </summary>
/// <param name="ChannelId">Идентификатор подтвержденного канала.</param>
/// <param name="PublisherUserId">Идентификатор паблишера, подтвердившего владение каналом.</param>
/// <param name="OccurredAtUtc">Время подтверждения владения в UTC.</param>
public sealed record ChannelOwnershipVerifiedV1(
    Guid ChannelId,
    Guid PublisherUserId,
    DateTimeOffset OccurredAtUtc
);
