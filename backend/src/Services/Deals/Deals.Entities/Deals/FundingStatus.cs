namespace Deals.Entities.Deals;

/// <summary>
/// Статус денег рекламодателя по сделке.
/// </summary>
public enum FundingStatus
{
    /// <summary>Резерв не создан.</summary>
    None = 1,

    /// <summary>Деньги зарезервированы в Payments.</summary>
    Reserved = 2,

    /// <summary>Резерв возвращен рекламодателю.</summary>
    Released = 3,

    /// <summary>Резерв списан в пользу паблишера.</summary>
    Captured = 4
}
