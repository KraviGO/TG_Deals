namespace Payments.Entities.Wallet;

/// <summary>
/// Тип движения денег в кошельке рекламодателя.
/// </summary>
public enum WalletTransactionType
{
    /// <summary>Успешное пополнение.</summary>
    TopUpSucceeded = 1,

    /// <summary>Создание резерва.</summary>
    ReserveCreated = 2,

    /// <summary>Возврат резерва.</summary>
    ReserveReleased = 3,

    /// <summary>Списание резерва.</summary>
    ReserveCaptured = 4,

    /// <summary>Внутреннее начисление.</summary>
    ManualCredit = 5,

    /// <summary>Вывод средств.</summary>
    WithdrawalCompleted = 6
}
