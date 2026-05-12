namespace Deals.UseCases.Deals.Disputes.ResolveDispute;

public sealed record ResolveDealDisputeCommand(
    Guid AdminUserId,
    Guid DealId,
    string Action,
    string? ResolutionNote
);
