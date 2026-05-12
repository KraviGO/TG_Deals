namespace Marketplace.Contracts.Common;

/// <summary>
/// Обертка события с payload и служебными метаданными.
/// Сервисы используют эти метаданные для трассировки, идемпотентной обработки и версионирования контракта.
/// </summary>
/// <typeparam name="TPayload">Тип payload события.</typeparam>
/// <param name="MessageId">Уникальный идентификатор сообщения для идемпотентной обработки.</param>
/// <param name="OccurredAt">Время возникновения события у источника.</param>
/// <param name="EventType">Имя события для маршрутизации и выбора обработчика.</param>
/// <param name="SchemaVersion">Версия схемы payload.</param>
/// <param name="CorrelationId">Идентификатор запроса, связывающий логи и события между сервисами. Null означает, что идентификатор не передан.</param>
/// <param name="Payload">Данные события.</param>
public sealed record EventEnvelope<TPayload>(
    Guid MessageId,
    DateTimeOffset OccurredAt,
    string EventType,
    int SchemaVersion,
    string? CorrelationId,
    TPayload Payload
);
