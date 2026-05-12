namespace Marketplace.Contracts.Publishers;

/// <summary>
/// Событие изменения статуса владения или модерации канала.
/// Подписчики обновляют доступность канала для каталога и сделок.
/// </summary>
/// <param name="ChannelId">Идентификатор канала внутри маркетплейса.</param>
/// <param name="PublisherUserId">Идентификатор владельца канала.</param>
/// <param name="OwnershipStatus">Новый статус владения или модерации канала.</param>
/// <param name="OccurredAtUtc">Время изменения статуса в UTC.</param>
public sealed record ChannelModerationChangedV1(
    Guid ChannelId,
    Guid PublisherUserId,
    string OwnershipStatus,
    DateTimeOffset OccurredAtUtc
);
