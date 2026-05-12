namespace Payments.Entities.TopUps;

/// <summary>
/// Статус пополнения кошелька.
/// </summary>
public enum TopUpStatus
{
    /// <summary>Платеж создан.</summary>
    Created = 1,

    /// <summary>Платеж успешно оплачен.</summary>
    Succeeded = 2,

    /// <summary>Платеж отменен.</summary>
    Canceled = 3
}
