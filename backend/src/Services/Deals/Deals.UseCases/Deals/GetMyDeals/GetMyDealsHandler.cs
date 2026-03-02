using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Deals.UseCases.Deals.GetMyDeals;

public sealed class GetMyDealsHandler
{
    private readonly IDealsDbContext _db;
    public GetMyDealsHandler(IDealsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<GetMyDealsResult>>> Handle(GetMyDealsQuery q, CancellationToken ct)
    {
        if (q.AdvertiserUserId == Guid.Empty)
            return Result<IReadOnlyList<GetMyDealsResult>>.Fail(Errors.Validation);

        var items = await _db.Deals
            .AsNoTracking()
            .Where(x => x.AdvertiserUserId == q.AdvertiserUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetMyDealsResult(x.DealId, x.ChannelId, x.Status.ToString(), x.DesiredPublishAtUtc, x.CreatedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<GetMyDealsResult>>.Ok(items);
    }
}
