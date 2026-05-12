namespace Payments.UseCases.PublisherWallet.WithdrawPublisherBalance;

public sealed record WithdrawPublisherBalanceResult(
    string Currency,
    decimal RequestedAmount,
    decimal WithdrawnAmount,
    decimal Available,
    decimal PaidOut,
    string? DestinationCardMask
);
