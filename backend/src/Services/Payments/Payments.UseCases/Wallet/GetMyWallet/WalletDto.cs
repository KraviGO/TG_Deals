namespace Payments.UseCases.Wallet.GetMyWallet;

public sealed record WalletDto(string Currency, decimal Available, decimal Reserved, decimal Total);
