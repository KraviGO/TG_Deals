using Deals.Entities.Deals;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Deals.UseCases.Deals.CancelDeal;

/// <summary>
/// Отменяет сделку по запросу рекламодателя.
/// </summary>
public sealed class CancelDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly IWalletClient _wallet;
    private readonly IClock _clock;
    private readonly ILogger<CancelDealHandler> _log;

    public CancelDealHandler(IDealsDbContext db, IWalletClient wallet, IClock clock, ILogger<CancelDealHandler> log)
    {
        _db = db;
        _wallet = wallet;
        _clock = clock;
        _log = log;
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

            if (deal.FundingStatus == FundingStatus.Reserved)
            {
                // Отмена возвращает активный резерв рекламодателю.
                await _wallet.ReleaseReservationAsync(deal.DealId, ct);
                deal.MarkFundingReleased(_clock.UtcNow);
            }
        }
        catch (HttpRequestException)
        {
            return Result.Fail("FundingReleaseFailed");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "CancelDeal failed for deal {DealId}", cmd.DealId);
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
