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

namespace Deals.UseCases.Deals.CreateDeal;

/// <summary>
/// Создает сделку рекламодателя.
/// Auto-mode сразу резервирует деньги и публикует пост.
/// </summary>
public sealed class CreateDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly ICatalogClient _catalog;
    private readonly IWalletClient _wallet;
    private readonly ITelegramPostPublisher _telegram;
    private readonly IClock _clock;
    private readonly ILogger<CreateDealHandler> _log;

    public CreateDealHandler(
        IDealsDbContext db,
        ICatalogClient catalog,
        IWalletClient wallet,
        ITelegramPostPublisher telegram,
        IClock clock,
        ILogger<CreateDealHandler> log)
    {
        _db = db;
        _catalog = catalog;
        _wallet = wallet;
        _telegram = telegram;
        _clock = clock;
        _log = log;
    }

    public async Task<Result<CreateDealResult>> Handle(CreateDealCommand cmd, CancellationToken ct)
    {
        if (cmd.AdvertiserUserId == Guid.Empty) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (cmd.ChannelId == Guid.Empty) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (string.IsNullOrWhiteSpace(cmd.PostText)) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<CreateDealResult>.Fail(Errors.Validation);
        if (!string.Equals(cmd.Currency, "RUB", StringComparison.OrdinalIgnoreCase))
            return Result<CreateDealResult>.Fail("CurrencyNotSupported");

        // Каталог задает владельца канала, статус верификации и режим приема заявок.
        var ch = await _catalog.GetChannelAsync(cmd.ChannelId, ct);
        if (ch is null) return Result<CreateDealResult>.Fail("ChannelNotFound");
        if (!string.Equals(ch.OwnershipStatus, "Verified", StringComparison.OrdinalIgnoreCase))
            return Result<CreateDealResult>.Fail("ChannelNotVerified");

        if (string.Equals(ch.IntakeMode, "Paused", StringComparison.OrdinalIgnoreCase))
            return Result<CreateDealResult>.Fail("ChannelPaused");

        var isAutoMode = string.Equals(ch.IntakeMode, "ActiveAuto", StringComparison.OrdinalIgnoreCase);
        var initialStatus = isAutoMode
            ? DealStatus.Accepted
            : DealStatus.PendingPublisherDecision;

        var deal = Deal.Create(
            channelId: cmd.ChannelId,
            publisherUserId: ch.PublisherUserId,
            advertiserUserId: cmd.AdvertiserUserId,
            postText: cmd.PostText,
            desiredPublishAtUtc: cmd.DesiredPublishAtUtc,
            amount: cmd.Amount,
            currency: cmd.Currency,
            initialStatus: initialStatus,
            nowUtc: _clock.UtcNow);

        if (isAutoMode)
        {
            // Auto-mode пропускает ручное решение паблишера.
            WalletReservationResult reserve;
            try
            {
                reserve = await _wallet.ReserveForDealAsync(
                    dealId: deal.DealId,
                    advertiserUserId: deal.AdvertiserUserId,
                    publisherUserId: deal.PublisherUserId,
                    amount: deal.Amount,
                    currency: deal.Currency,
                    ct: ct);
            }
            catch (HttpRequestException)
            {
                return Result<CreateDealResult>.Fail("FundingReserveFailed");
            }

            deal.MarkFundingReserved(reserve.ReservationId, _clock.UtcNow);

            try
            {
                var published = await _telegram.PublishTextAsync(ch.TelegramChannelId, deal.PostText, ct);
                if (string.IsNullOrWhiteSpace(published.PostUrl))
                {
                    _log.LogWarning("Telegram auto publish returned empty post URL for deal {DealId}", deal.DealId);
                    await ReleaseReservedFundingAfterPublishFailure(deal.DealId, ct);
                    return Result<CreateDealResult>.Fail(Errors.TelegramPublishFailed);
                }

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
                _log.LogError(ex, "Telegram auto publish failed for auto-mode deal {DealId}", deal.DealId);
                await ReleaseReservedFundingAfterPublishFailure(deal.DealId, ct);
                return Result<CreateDealResult>.Fail(Errors.TelegramPublishFailed);
            }
        }

        await _db.AddDealAsync(deal, ct);
        await _db.SaveChangesAsync(ct);

        return Result<CreateDealResult>.Ok(new CreateDealResult(
            deal.DealId,
            deal.Status.ToString(),
            deal.FundingStatus.ToString(),
            deal.ReservationId));
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
