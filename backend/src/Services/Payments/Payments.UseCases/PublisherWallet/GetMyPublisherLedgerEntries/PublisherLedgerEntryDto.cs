namespace Payments.UseCases.PublisherWallet.GetMyPublisherLedgerEntries;

public sealed record PublisherLedgerEntryDto(
    Guid EntryId,
    Guid DealId,
    decimal GrossAmount,
    decimal PlatformFeeAmount,
    decimal PublisherAmount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AvailableAt
);
