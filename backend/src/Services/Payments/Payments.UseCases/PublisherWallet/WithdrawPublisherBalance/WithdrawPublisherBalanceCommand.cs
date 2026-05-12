namespace Payments.UseCases.PublisherWallet.WithdrawPublisherBalance;

public sealed record WithdrawPublisherBalanceCommand(
    Guid PublisherUserId,
    IReadOnlyList<Guid>? EntryIds,
    decimal? Amount,
    string? CardNumber
);
