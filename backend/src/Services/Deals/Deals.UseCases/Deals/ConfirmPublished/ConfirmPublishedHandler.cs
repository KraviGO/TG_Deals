using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Catalog;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Telegram;
using Marketplace.Kernel.Results;
using Deals.UseCases.Common;
using Microsoft.Extensions.Logging;

namespace Deals.UseCases.Deals.ConfirmPublished;

/// <summary>
/// Фиксирует опубликованный пост и переводит сделку к подтверждению рекламодателем.
/// </summary>
public sealed class ConfirmPublishedHandler
{
    private readonly IDealsDbContext _db;
    private readonly ICatalogClient _catalog;
    private readonly ITelegramPostPublisher _telegram;
    private readonly IClock _clock;
    private readonly ILogger<ConfirmPublishedHandler> _log;

    public ConfirmPublishedHandler(
        IDealsDbContext db,
        ICatalogClient catalog,
        ITelegramPostPublisher telegram,
        IClock clock,
        ILogger<ConfirmPublishedHandler> log)
    {
        _db = db;
        _catalog = catalog;
        _telegram = telegram;
        _clock = clock;
        _log = log;
    }

    public async Task<Result> Handle(ConfirmPublishedCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result.Fail(Errors.NotFound);

        if (deal.PublisherUserId != cmd.PublisherUserId)
            return Result.Fail(Errors.Forbidden);

        var now = _clock.UtcNow;
        var postUrl = cmd.PostUrl;
        var publishedAtUtc = cmd.PublishedAtUtc;

        if (string.IsNullOrWhiteSpace(postUrl))
        {
            try
            {
                // Без PostUrl сервис публикует текст через telegram-publisher.
                var channel = await _catalog.GetChannelAsync(deal.ChannelId, ct);
                if (channel is null) return Result.Fail(Errors.NotFound);

                var published = await _telegram.PublishTextAsync(channel.TelegramChannelId, deal.PostText, ct);
                postUrl = published.PostUrl;
                publishedAtUtc = published.PostedAtUtc;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Telegram auto publish failed for deal {DealId}", cmd.DealId);
                return Result.Fail(Errors.TelegramPublishFailed);
            }
        }

        if (string.IsNullOrWhiteSpace(postUrl))
        {
            _log.LogWarning("Telegram auto publish returned empty post URL for deal {DealId}", cmd.DealId);
            return Result.Fail(Errors.TelegramPublishFailed);
        }

        try
        {
            deal.PublisherConfirmPublished(postUrl, publishedAtUtc ?? now, cmd.PublisherComment, now);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "ConfirmPublished failed for deal {DealId}", cmd.DealId);
            return Result.Fail(Errors.InvalidState);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
