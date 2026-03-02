using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;

namespace Deals.UseCases.Deals.DecideDeal;

public sealed class PublisherDecideDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly IClock _clock;

    public PublisherDecideDealHandler(IDealsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(PublisherDecideDealCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        if (deal.PublisherUserId != cmd.PublisherUserId)
            return Result.Fail(Errors.Forbidden);

        try
        {
            if (cmd.Accept) deal.PublisherAccept(_clock.UtcNow);
            else deal.PublisherReject(_clock.UtcNow);
        }
        catch
        {
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
