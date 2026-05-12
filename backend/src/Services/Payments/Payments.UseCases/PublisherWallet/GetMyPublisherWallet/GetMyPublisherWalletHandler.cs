using Microsoft.EntityFrameworkCore;
using Payments.Entities.PublisherLedger;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.PublisherWallet.GetMyPublisherWallet;

public sealed class GetMyPublisherWalletHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public GetMyPublisherWalletHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<PublisherWalletDto>> Handle(GetMyPublisherWalletQuery q, CancellationToken ct)
    {
        if (q.PublisherUserId == Guid.Empty)
            return Result<PublisherWalletDto>.Fail(Errors.Validation);

        var wallet = await _db.PublisherWallets.FirstOrDefaultAsync(x => x.PublisherUserId == q.PublisherUserId, ct);
        if (wallet is null)
        {
            wallet = global::Payments.Entities.PublisherLedger.PublisherWallet.Create(q.PublisherUserId, "RUB", _clock.UtcNow);
            await _db.AddPublisherWalletAsync(wallet, ct);
            await _db.SaveChangesAsync(ct);
        }

        return Result<PublisherWalletDto>.Ok(new PublisherWalletDto(
            wallet.Currency,
            wallet.Available,
            wallet.PaidOut,
            wallet.Available + wallet.PaidOut));
    }
}
