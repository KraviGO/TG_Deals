namespace Payments.Entities.Wallet;

public enum WalletTransactionType
{
    TopUpSucceeded = 1,
    ReserveCreated = 2,
    ReserveReleased = 3,
    ReserveCaptured = 4
}
