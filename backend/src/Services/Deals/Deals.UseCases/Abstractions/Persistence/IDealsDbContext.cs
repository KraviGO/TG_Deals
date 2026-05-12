using Deals.Entities.Deals;
using Deals.Entities.Disputes;

namespace Deals.UseCases.Abstractions.Persistence;

public interface IDealsDbContext
{
    IQueryable<Deal> Deals { get; }
    IQueryable<DealDispute> Disputes { get; }

    Task AddDealAsync(Deal deal, CancellationToken ct);
    Task AddDisputeAsync(DealDispute dispute, CancellationToken ct);
    Task<Deal?> FindDealAsync(Guid dealId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
