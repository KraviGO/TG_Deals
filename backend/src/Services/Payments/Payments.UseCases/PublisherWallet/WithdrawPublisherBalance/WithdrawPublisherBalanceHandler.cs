using Microsoft.EntityFrameworkCore;
using Payments.Entities.PublisherLedger;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;
using PublisherWalletEntity = Payments.Entities.PublisherLedger.PublisherWallet;

namespace Payments.UseCases.PublisherWallet.WithdrawPublisherBalance;

/// <summary>
/// Фиксирует вывод средств паблишера.
/// </summary>
public sealed class WithdrawPublisherBalanceHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public WithdrawPublisherBalanceHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<WithdrawPublisherBalanceResult>> Handle(WithdrawPublisherBalanceCommand cmd, CancellationToken ct)
    {
        if (cmd.PublisherUserId == Guid.Empty) return Result<WithdrawPublisherBalanceResult>.Fail(Errors.Validation);
        if (!cmd.Amount.HasValue || cmd.Amount.Value <= 0)
            return Result<WithdrawPublisherBalanceResult>.Fail(Errors.Validation);

        var cardDigits = NormalizeCardDigits(cmd.CardNumber);
        if (cardDigits.Length < 12 || cardDigits.Length > 19)
            return Result<WithdrawPublisherBalanceResult>.Fail(Errors.Validation);

        var requestedAmount = decimal.Round(cmd.Amount.Value, 2, MidpointRounding.AwayFromZero);
        var now = _clock.UtcNow;
        var wallet = await GetOrCreateWallet(cmd.PublisherUserId, now, ct);

        if (wallet.Available < requestedAmount)
            return Result<WithdrawPublisherBalanceResult>.Fail("InsufficientFunds");

        var cardMask = MaskCard(cardDigits);

        try
        {
            // MVP сразу переносит сумму из Available в PaidOut.
            wallet.MarkPaidOut(requestedAmount, now);
        }
        catch (InvalidOperationException ex) when (ex.Message is "InsufficientAvailableAmount")
        {
            return Result<WithdrawPublisherBalanceResult>.Fail("InsufficientFunds");
        }

        await _db.AddPublisherWithdrawalAsync(PublisherWithdrawal.Create(
            cmd.PublisherUserId,
            requestedAmount,
            wallet.Currency,
            cardMask,
            now), ct);

        await _db.SaveChangesAsync(ct);

        return Result<WithdrawPublisherBalanceResult>.Ok(new WithdrawPublisherBalanceResult(
            wallet.Currency,
            requestedAmount,
            requestedAmount,
            wallet.Available,
            wallet.PaidOut,
            cardMask));
    }

    private async Task<global::Payments.Entities.PublisherLedger.PublisherWallet> GetOrCreateWallet(
        Guid publisherUserId,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var wallet = await _db.PublisherWallets.FirstOrDefaultAsync(x => x.PublisherUserId == publisherUserId, ct);
        if (wallet is not null) return wallet;

        wallet = PublisherWalletEntity.Create(publisherUserId, "RUB", now);
        await _db.AddPublisherWalletAsync(wallet, ct);
        return wallet;
    }

    private static string NormalizeCardDigits(string? rawCard)
    {
        if (string.IsNullOrWhiteSpace(rawCard)) return string.Empty;
        return new string(rawCard.Where(char.IsDigit).ToArray());
    }

    private static string MaskCard(string cardDigits)
    {
        // В БД хранится только маска карты.
        if (cardDigits.Length < 4) return "****";
        var last4 = cardDigits[^4..];
        return $"**** **** **** {last4}";
    }
}
