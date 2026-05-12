using Deals.Entities.Deals;
using Deals.UseCases.Abstractions.Catalog;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Telegram;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Deals.UseCases.Deals.DecideDeal;

/// <summary>
/// Обрабатывает ручное решение паблишера.
/// Accept резервирует деньги и публикует пост, reject закрывает заявку.
/// </summary>
public sealed class PublisherDecideDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly ICatalogClient _catalog;
    private readonly IWalletClient _wallet;
    private readonly ITelegramPostPublisher _telegram;
    private readonly IClock _clock;
    private readonly ILogger<PublisherDecideDealHandler> _log;

    public PublisherDecideDealHandler(
        IDealsDbContext db,
        ICatalogClient catalog,
        IWalletClient wallet,
        ITelegramPostPublisher telegram,
        IClock clock,
        ILogger<PublisherDecideDealHandler> log)
    {
        _db = db;
        _catalog = catalog;
        _wallet = wallet;
        _telegram = telegram;
        _clock = clock;
        _log = log;
    }

    public async Task<Result> Handle(PublisherDecideDealCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        if (deal.PublisherUserId != cmd.PublisherUserId)
            return Result.Fail(Errors.Forbidden);

        try
        {
            if (cmd.Accept)
            {
                // Ручной режим резервирует деньги после согласия паблишера.
                var reserve = await _wallet.ReserveForDealAsync(
                    dealId: deal.DealId,
                    advertiserUserId: deal.AdvertiserUserId,
                    publisherUserId: deal.PublisherUserId,
                    amount: deal.Amount,
                    currency: deal.Currency,
                    ct: ct);

                deal.PublisherAccept(_clock.UtcNow);
                deal.MarkFundingReserved(reserve.ReservationId, _clock.UtcNow);

                try
                {
                    var channel = await _catalog.GetChannelAsync(deal.ChannelId, ct);
                    if (channel is null)
                    {
                        await ReleaseReservedFundingAfterPublishFailure(deal.DealId, ct);
                        return Result.Fail(Errors.NotFound);
                    }

                    var published = await _telegram.PublishTextAsync(channel.TelegramChannelId, deal.PostText, ct);
                    if (string.IsNullOrWhiteSpace(published.PostUrl))
                    {
                        _log.LogWarning("Telegram auto publish returned empty post URL for deal {DealId}", deal.DealId);
                        await ReleaseReservedFundingAfterPublishFailure(deal.DealId, ct);
                        return Result.Fail(Errors.TelegramPublishFailed);
                    }

                    // Telegram вернул ссылку, сделка переходит к подтверждению рекламодателем.
                    deal.PublisherConfirmPublished(
                        published.PostUrl,
                        published.PostedAtUtc,
                        publisherComment: null,
                        nowUtc: _clock.UtcNow);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Telegram auto publish failed for deal {DealId}", deal.DealId);
                    await ReleaseReservedFundingAfterPublishFailure(deal.DealId, ct);
                    return Result.Fail(Errors.TelegramPublishFailed);
                }
            }
            else
            {
                deal.PublisherReject(_clock.UtcNow);

                if (deal.FundingStatus == FundingStatus.Reserved)
                {
                    // Отказ паблишера возвращает созданный резерв рекламодателю.
                    await _wallet.ReleaseReservationAsync(deal.DealId, ct);
                    deal.MarkFundingReleased(_clock.UtcNow);
                }
            }
        }
        catch (HttpRequestException)
        {
            return Result.Fail("FundingReserveFailed");
        }
        catch
        {
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private async Task ReleaseReservedFundingAfterPublishFailure(Guid dealId, CancellationToken ct)
    {
        try
        {
            // Публикация не состоялась, поэтому резерв возвращается рекламодателю.
            await _wallet.ReleaseReservationAsync(dealId, ct);
        }
        catch (Exception releaseEx) when (releaseEx is not OperationCanceledException)
        {
            _log.LogError(releaseEx, "Failed to release reserved funding after Telegram publish failure for deal {DealId}", dealId);
        }
    }
}
