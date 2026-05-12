namespace Payments.UseCases.Wallet.WithdrawFromWallet;

public sealed record WithdrawFromWalletCommand(Guid UserId, decimal Amount, string Currency);
