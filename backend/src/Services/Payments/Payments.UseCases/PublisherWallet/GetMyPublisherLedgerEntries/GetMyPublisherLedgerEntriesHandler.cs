using Microsoft.EntityFrameworkCore;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.PublisherWallet.GetMyPublisherLedgerEntries;

public sealed class GetMyPublisherLedgerEntriesHandler
{
    private readonly IPaymentsDbContext _db;

    public GetMyPublisherLedgerEntriesHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<PublisherLedgerEntryDto>>> Handle(GetMyPublisherLedgerEntriesQuery q, CancellationToken ct)
    {
        if (q.PublisherUserId == Guid.Empty)
            return Result<IReadOnlyList<PublisherLedgerEntryDto>>.Fail(Errors.Validation);

        var items = await _db.PublisherLedgerEntries
            .AsNoTracking()
            .Where(x => x.PublisherUserId == q.PublisherUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PublisherLedgerEntryDto(
                x.EntryId,
                x.DealId,
                x.GrossAmount,
                x.PlatformFeeAmount,
                x.PublisherAmount,
                x.Currency,
                x.Status.ToString(),
                x.CreatedAt,
                x.AvailableAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<PublisherLedgerEntryDto>>.Ok(items);
    }
}
