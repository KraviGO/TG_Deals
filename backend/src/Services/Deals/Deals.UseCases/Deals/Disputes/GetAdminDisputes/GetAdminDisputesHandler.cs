using Deals.Entities.Disputes;
using Deals.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Deals.UseCases.Deals.Disputes.GetAdminDisputes;

public sealed class GetAdminDisputesHandler
{
    private readonly IDealsDbContext _db;

    public GetAdminDisputesHandler(IDealsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<GetAdminDisputesResult>>> Handle(GetAdminDisputesQuery q, CancellationToken ct)
    {
        var limit = Math.Clamp(q.Limit <= 0 ? 100 : q.Limit, 1, 500);
        var offset = Math.Max(0, q.Offset);

        var statusFilter = q.Status?.Trim().ToLowerInvariant();
        if (statusFilter is not null && statusFilter is not ("open" or "resolved" or "all"))
            return Result<IReadOnlyList<GetAdminDisputesResult>>.Fail(Errors.Validation);

        var query =
            from dispute in _db.Disputes.AsNoTracking()
            join deal in _db.Deals.AsNoTracking() on dispute.DealId equals deal.DealId
            select new { dispute, deal };

        if (statusFilter == "open")
            query = query.Where(x => x.dispute.Status == DisputeStatus.Open);
        else if (statusFilter == "resolved")
            query = query.Where(x => x.dispute.Status == DisputeStatus.Resolved);

        var items = await query
            .OrderByDescending(x => x.dispute.Status == DisputeStatus.Open)
            .ThenByDescending(x => x.dispute.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .Select(x => new GetAdminDisputesResult(
                x.dispute.DisputeId,
                x.dispute.DealId,
                x.dispute.Status.ToString(),
                x.dispute.Reason,
                x.dispute.OpenedByUserId,
                x.dispute.OpenedByRole,
                x.dispute.CreatedAt,
                x.dispute.ResolvedByUserId,
                x.dispute.ResolutionAction.HasValue ? x.dispute.ResolutionAction.Value.ToString() : null,
                x.dispute.ResolutionNote,
                x.dispute.ResolvedAt,
                x.deal.ChannelId,
                x.deal.AdvertiserUserId,
                x.deal.PublisherUserId,
                x.deal.Status.ToString(),
                x.deal.FundingStatus.ToString(),
                x.deal.Amount,
                x.deal.Currency,
                x.deal.PostUrl,
                x.deal.PublishedAtUtc))
            .ToListAsync(ct);

        return Result<IReadOnlyList<GetAdminDisputesResult>>.Ok(items);
    }
}

