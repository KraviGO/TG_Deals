namespace Payments.UseCases.PublisherWallet.GetMyPublisherWallet;

public sealed record PublisherWalletDto(
    string Currency,
    decimal Available,
    decimal PaidOut,
    decimal TotalAccrued
);
