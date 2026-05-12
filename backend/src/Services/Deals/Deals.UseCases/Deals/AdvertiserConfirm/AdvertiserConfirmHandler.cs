using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using System.Net.Http;

namespace Deals.UseCases.Deals.AdvertiserConfirm;

/// <summary>
/// Подтверждает публикацию рекламодателем и списывает резерв.
/// </summary>
public sealed class AdvertiserConfirmHandler
{
    private readonly IDealsDbContext _db;
    private readonly IWalletClient _wallet;
    private readonly IClock _clock;

    public AdvertiserConfirmHandler(IDealsDbContext db, IWalletClient wallet, IClock clock)
    {
        _db = db;
        _wallet = wallet;
        _clock = clock;
    }

    public async Task<Result> Handle(AdvertiserConfirmCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        if (deal.AdvertiserUserId != cmd.AdvertiserUserId)
            return Result.Fail(Errors.Forbidden);

        try
        {
            // Capture переводит зарезервированные деньги паблишеру.
            await _wallet.CaptureReservationAsync(deal.DealId, ct);
            deal.AdvertiserConfirmAndComplete(_clock.UtcNow);
        }
        catch (HttpRequestException)
        {
            return Result.Fail("FundingCaptureFailed");
        }
        catch
        {
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
