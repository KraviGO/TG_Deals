using Microsoft.EntityFrameworkCore;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.GetMyWalletTransactions;

public sealed class GetMyWalletTransactionsHandler
{
    private readonly IPaymentsDbContext _db;

    public GetMyWalletTransactionsHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<WalletTransactionDto>>> Handle(GetMyWalletTransactionsQuery q, CancellationToken ct)
    {
        if (q.UserId == Guid.Empty) return Result<IReadOnlyList<WalletTransactionDto>>.Fail(Errors.Validation);

        var limit = q.Limit <= 0 ? 100 : Math.Min(q.Limit, 500);

        var rows = await _db.WalletTransactions
            .AsNoTracking()
            .Where(x => x.UserId == q.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .Select(x => new WalletTransactionDto(
                x.TxId,
                x.Type.ToString(),
                x.Amount,
                x.Currency,
                x.DealId,
                x.TopUpId,
                x.CreatedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<WalletTransactionDto>>.Ok(rows);
    }
}
