namespace Deals.UseCases.Deals.Disputes.OpenDispute;

public sealed record OpenDealDisputeCommand(
    Guid UserId,
    string UserRole,
    Guid DealId,
    string Reason
);
