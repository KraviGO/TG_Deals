using Deals.Entities.Disputes;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Deals.UseCases.Deals.Disputes.ResolveDispute;

/// <summary>
/// Закрывает спор решением администратора.
/// </summary>
public sealed class ResolveDealDisputeHandler
{
    private readonly IDealsDbContext _db;
    private readonly IWalletClient _wallet;
    private readonly IClock _clock;
    private readonly ILogger<ResolveDealDisputeHandler> _log;

    public ResolveDealDisputeHandler(IDealsDbContext db, IWalletClient wallet, IClock clock, ILogger<ResolveDealDisputeHandler> log)
    {
        _db = db;
        _wallet = wallet;
        _clock = clock;
        _log = log;
    }

    public async Task<Result> Handle(ResolveDealDisputeCommand cmd, CancellationToken ct)
    {
        if (cmd.AdminUserId == Guid.Empty || cmd.DealId == Guid.Empty || string.IsNullOrWhiteSpace(cmd.Action))
            return Result.Fail(Errors.Validation);

        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        var dispute = await _db.Disputes.FirstOrDefaultAsync(x => x.DealId == cmd.DealId && x.Status == DisputeStatus.Open, ct);
        if (dispute is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;
        var action = cmd.Action.Trim().ToLowerInvariant();

        try
        {
            if (action == "capture")
            {
                // Capture завершает спор в пользу паблишера.
                await _wallet.CaptureReservationAsync(deal.DealId, ct);
                deal.ResolveAfterCapture(now);
                dispute.Resolve(cmd.AdminUserId, DisputeResolutionAction.Capture, cmd.ResolutionNote, now);
            }
            else if (action == "release")
            {
                // Release завершает спор возвратом денег рекламодателю.
                await _wallet.ReleaseReservationAsync(deal.DealId, ct);
                deal.ResolveAfterRelease(now);
                dispute.Resolve(cmd.AdminUserId, DisputeResolutionAction.Release, cmd.ResolutionNote, now);
            }
            else
            {
                return Result.Fail(Errors.Validation);
            }
        }
        catch (HttpRequestException)
        {
            return Result.Fail("FundingResolutionFailed");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "ResolveDispute failed for deal {DealId} action {Action}", cmd.DealId, cmd.Action);
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
