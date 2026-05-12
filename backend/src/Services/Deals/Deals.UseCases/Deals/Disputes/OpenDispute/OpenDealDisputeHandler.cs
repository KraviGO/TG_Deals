using Deals.Entities.Disputes;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Deals.UseCases.Deals.Disputes.OpenDispute;

/// <summary>
/// Открывает спор от участника сделки.
/// </summary>
public sealed class OpenDealDisputeHandler
{
    private readonly IDealsDbContext _db;
    private readonly IClock _clock;
    private readonly ILogger<OpenDealDisputeHandler> _log;

    public OpenDealDisputeHandler(IDealsDbContext db, IClock clock, ILogger<OpenDealDisputeHandler> log)
    {
        _db = db;
        _clock = clock;
        _log = log;
    }

    public async Task<Result<OpenDealDisputeResult>> Handle(OpenDealDisputeCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId == Guid.Empty || cmd.DealId == Guid.Empty || string.IsNullOrWhiteSpace(cmd.Reason))
            return Result<OpenDealDisputeResult>.Fail(Errors.Validation);

        var role = (cmd.UserRole ?? string.Empty).Trim();
        if (role != "Advertiser" && role != "Publisher")
            return Result<OpenDealDisputeResult>.Fail(Errors.Forbidden);

        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result<OpenDealDisputeResult>.Fail(Errors.NotFound);

        if (role == "Advertiser" && deal.AdvertiserUserId != cmd.UserId)
            return Result<OpenDealDisputeResult>.Fail(Errors.Forbidden);

        if (role == "Publisher" && deal.PublisherUserId != cmd.UserId)
            return Result<OpenDealDisputeResult>.Fail(Errors.Forbidden);

        var existing = await _db.Disputes.FirstOrDefaultAsync(x => x.DealId == cmd.DealId && x.Status == DisputeStatus.Open, ct);
        if (existing is not null)
            return Result<OpenDealDisputeResult>.Fail(Errors.InvalidState);

        var now = _clock.UtcNow;

        try
        {
            // Открытый спор блокирует обычное завершение сделки.
            deal.MarkDisputed(now);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "MarkDisputed failed for deal {DealId}", cmd.DealId);
            return Result<OpenDealDisputeResult>.Fail(Errors.InvalidState);
        }

        var dispute = DealDispute.Open(
            dealId: cmd.DealId,
            openedByUserId: cmd.UserId,
            openedByRole: role,
            reason: cmd.Reason,
            nowUtc: now);

        await _db.AddDisputeAsync(dispute, ct);

        await _db.SaveChangesAsync(ct);

        return Result<OpenDealDisputeResult>.Ok(new OpenDealDisputeResult(dispute.DisputeId, dispute.Status.ToString()));
    }
}
