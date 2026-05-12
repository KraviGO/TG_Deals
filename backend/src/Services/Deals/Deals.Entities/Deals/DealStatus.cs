namespace Deals.Entities.Deals;

/// <summary>
/// Статус сделки в бизнес-процессе размещения.
/// </summary>
public enum DealStatus
{
    /// <summary>Ожидает решения паблишера.</summary>
    PendingPublisherDecision = 1,

    /// <summary>Паблишер принял заявку.</summary>
    Accepted = 2,

    /// <summary>Паблишер отклонил заявку.</summary>
    Rejected = 3,

    /// <summary>Рекламодатель отменил сделку.</summary>
    CanceledByAdvertiser = 4,

    /// <summary>Деньги рекламодателя зарезервированы.</summary>
    FundingReserved = 5,

    /// <summary>Сделка готова к публикации.</summary>
    ReadyToPublish = 6,

    /// <summary>Пост опубликован и ждет подтверждения рекламодателя.</summary>
    PublishedPendingConfirm = 7,

    /// <summary>Сделка завершена, резерв списан.</summary>
    Completed = 8,

    /// <summary>По сделке открыт спор.</summary>
    Disputed = 9,

    /// <summary>Спор решен администратором.</summary>
    Resolved = 10,

    /// <summary>Сделка создана.</summary>
    Created = 11
}
