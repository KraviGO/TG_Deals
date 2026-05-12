using Microsoft.EntityFrameworkCore;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.TopUps.GetMyTopUps;

public sealed class GetMyTopUpsHandler
{
    private readonly IPaymentsDbContext _db;

    public GetMyTopUpsHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<TopUpHistoryDto>>> Handle(GetMyTopUpsQuery q, CancellationToken ct)
    {
        if (q.UserId == Guid.Empty) return Result<IReadOnlyList<TopUpHistoryDto>>.Fail(Errors.Validation);

        var limit = q.Limit <= 0 ? 100 : Math.Min(q.Limit, 500);

        var rows = await _db.TopUps
            .AsNoTracking()
            .Where(x => x.UserId == q.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .Select(x => new TopUpHistoryDto(
                x.TopUpId,
                x.YooKassaPaymentId,
                x.Amount,
                x.Currency,
                x.Status.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<TopUpHistoryDto>>.Ok(rows);
    }
}
