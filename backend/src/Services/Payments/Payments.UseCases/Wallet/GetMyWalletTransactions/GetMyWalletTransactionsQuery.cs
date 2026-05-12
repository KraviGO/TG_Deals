namespace Payments.UseCases.Wallet.GetMyWalletTransactions;

public sealed record GetMyWalletTransactionsQuery(Guid UserId, int Limit = 100);
