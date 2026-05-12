namespace Payments.Entities.Wallet;

/// <summary>
/// Статус резерва по сделке.
/// </summary>
public enum ReservationStatus
{
    /// <summary>Деньги зарезервированы.</summary>
    Reserved = 1,

    /// <summary>Деньги возвращены рекламодателю.</summary>
    Released = 2,

    /// <summary>Деньги списаны в пользу паблишера.</summary>
    Captured = 3
}
