using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Deals.UseCases.Deals.GetPublisherDeals;

public sealed class GetPublisherDealsHandler
{
    private readonly IDealsDbContext _db;
    public GetPublisherDealsHandler(IDealsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<GetPublisherDealsResult>>> Handle(GetPublisherDealsQuery q, CancellationToken ct)
    {
        if (q.PublisherUserId == Guid.Empty)
            return Result<IReadOnlyList<GetPublisherDealsResult>>.Fail(Errors.Validation);

        var items = await _db.Deals
            .AsNoTracking()
            .Where(x => x.PublisherUserId == q.PublisherUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetPublisherDealsResult(
                x.DealId, x.ChannelId, x.AdvertiserUserId, x.Status.ToString(), x.DesiredPublishAtUtc, x.CreatedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<GetPublisherDealsResult>>.Ok(items);
    }
}
