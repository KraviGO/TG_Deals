namespace Marketplace.Messaging.Outbox;

/// <summary>
/// Запись outbox-таблицы для публикации интеграционного события.
/// Сервис сохраняет запись в своей транзакции, publisher отправляет ее в брокер.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// Идентификатор outbox-записи.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Время возникновения события в домене.
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// Имя события для маршрутизации и выбора обработчика.
    /// </summary>
    public string EventType { get; set; } = default!;

    /// <summary>
    /// Версия схемы события.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// RabbitMQ exchange для публикации сообщения.
    /// </summary>
    public string Exchange { get; set; } = "marketplace.events";

    /// <summary>
    /// Routing key для topic exchange.
    /// </summary>
    public string RoutingKey { get; set; } = default!;

    /// <summary>
    /// Payload события в JSON-формате.
    /// </summary>
    public string PayloadJson { get; set; } = default!;

    /// <summary>
    /// Текущий статус публикации.
    /// </summary>
    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;

    /// <summary>
    /// Количество попыток публикации.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Текст последней ошибки публикации. Null означает, что ошибка не записана.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Время успешной публикации в брокер. Null означает, что сообщение не опубликовано.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// Время следующей попытки публикации. Null означает, что попытка не запланирована.
    /// </summary>
    public DateTimeOffset? NextAttemptAt { get; set; }
}
