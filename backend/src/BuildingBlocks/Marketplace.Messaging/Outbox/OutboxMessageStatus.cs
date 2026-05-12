namespace Marketplace.Messaging.Outbox;

/// <summary>
/// Статус публикации outbox-сообщения.
/// Числовые значения закрепляют формат хранения в БД.
/// </summary>
public enum OutboxMessageStatus
{
    /// <summary>
    /// Сообщение ожидает публикации.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Сообщение успешно опубликовано в брокер.
    /// </summary>
    Published = 2,

    /// <summary>
    /// Последняя попытка публикации завершилась ошибкой.
    /// </summary>
    Failed = 3
}
