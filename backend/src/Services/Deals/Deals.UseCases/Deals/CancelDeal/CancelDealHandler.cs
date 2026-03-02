using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;

namespace Deals.UseCases.Deals.CancelDeal;

public sealed class CancelDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly IClock _clock;

    public CancelDealHandler(IDealsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(CancelDealCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        if (deal.AdvertiserUserId != cmd.AdvertiserUserId)
            return Result.Fail(Errors.Forbidden);

        try
        {
            deal.CancelByAdvertiser(_clock.UtcNow);
        }
        catch
        {
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
