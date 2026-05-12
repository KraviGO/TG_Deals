namespace Deals.UseCases.Deals.Disputes.GetAdminDisputes;

public sealed record GetAdminDisputesQuery(
    string? Status,
    int Limit = 100,
    int Offset = 0
);

