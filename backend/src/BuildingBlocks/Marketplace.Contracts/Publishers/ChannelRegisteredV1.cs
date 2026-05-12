namespace Marketplace.Contracts.Publishers;

/// <summary>
/// Событие регистрации Telegram-канала паблишером.
/// Передает публичные данные канала и начальные статусы подписчикам.
/// </summary>
/// <param name="ChannelId">Идентификатор канала внутри маркетплейса.</param>
/// <param name="PublisherUserId">Идентификатор паблишера, добавившего канал.</param>
/// <param name="TelegramChannelId">Идентификатор или username канала в Telegram.</param>
/// <param name="Title">Название канала.</param>
/// <param name="Topic">Тематика канала. Null означает, что тематика не задана.</param>
/// <param name="Language">Язык основного контента канала. Null означает, что язык не задан.</param>
/// <param name="PricePerPostRub">Цена публикации в рублях на момент регистрации. Null означает, что цена не задана.</param>
/// <param name="IntakeMode">Режим приема сделок по каналу.</param>
/// <param name="OwnershipStatus">Начальный статус подтверждения владения каналом.</param>
/// <param name="OccurredAtUtc">Время регистрации канала в UTC.</param>
public sealed record ChannelRegisteredV1(
    Guid ChannelId,
    Guid PublisherUserId,
    string TelegramChannelId,
    string Title,
    string? Topic,
    string? Language,
    decimal? PricePerPostRub,
    string IntakeMode,
    string OwnershipStatus,
    DateTimeOffset OccurredAtUtc
);
