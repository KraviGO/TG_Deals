using Deals.Entities.Deals;
using Deals.UseCases.Abstractions.Catalog;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;

namespace Deals.UseCases.Deals.CreateDeal;

public sealed class CreateDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly ICatalogClient _catalog;
    private readonly IClock _clock;

    public CreateDealHandler(IDealsDbContext db, ICatalogClient catalog, IClock clock)
    {
        _db = db;
        _catalog = catalog;
        _clock = clock;
    }

    public async Task<Result<CreateDealResult>> Handle(CreateDealCommand cmd, CancellationToken ct)
    {
        if (cmd.AdvertiserUserId == Guid.Empty) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (cmd.ChannelId == Guid.Empty) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (string.IsNullOrWhiteSpace(cmd.PostText)) return Result<CreateDealResult>.Fail(Errors.Validation);

        var ch = await _catalog.GetChannelAsync(cmd.ChannelId, ct);
        if (ch is null) return Result<CreateDealResult>.Fail("ChannelNotFound");
        if (!string.Equals(ch.OwnershipStatus, "Verified", StringComparison.OrdinalIgnoreCase))
            return Result<CreateDealResult>.Fail("ChannelNotVerified");

        if (string.Equals(ch.IntakeMode, "Paused", StringComparison.OrdinalIgnoreCase))
            return Result<CreateDealResult>.Fail("ChannelPaused");

        var initialStatus = string.Equals(ch.IntakeMode, "ActiveAuto", StringComparison.OrdinalIgnoreCase)
            ? DealStatus.Accepted
            : DealStatus.PendingPublisherDecision;

        var deal = Deal.Create(
            channelId: cmd.ChannelId,
            publisherUserId: ch.PublisherUserId,
            advertiserUserId: cmd.AdvertiserUserId,
            postText: cmd.PostText,
            desiredPublishAtUtc: cmd.DesiredPublishAtUtc,
            initialStatus: initialStatus,
            nowUtc: _clock.UtcNow);

        await _db.AddDealAsync(deal, ct);
        await _db.SaveChangesAsync(ct);

        return Result<CreateDealResult>.Ok(new CreateDealResult(deal.DealId, deal.Status.ToString()));
    }
}
