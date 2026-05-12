namespace Deals.UseCases.Deals.Disputes.GetAdminDisputes;

public sealed record GetAdminDisputesResult(
    Guid DisputeId,
    Guid DealId,
    string DisputeStatus,
    string Reason,
    Guid OpenedByUserId,
    string OpenedByRole,
    DateTimeOffset DisputeCreatedAt,
    Guid? ResolvedByUserId,
    string? ResolutionAction,
    string? ResolutionNote,
    DateTimeOffset? ResolvedAt,
    Guid ChannelId,
    Guid AdvertiserUserId,
    Guid PublisherUserId,
    string DealStatus,
    string FundingStatus,
    decimal Amount,
    string Currency,
    string? PostUrl,
    DateTimeOffset? PublishedAtUtc
);

