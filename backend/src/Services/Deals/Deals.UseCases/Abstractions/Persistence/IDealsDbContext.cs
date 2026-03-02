using Deals.Entities.Deals;

namespace Deals.UseCases.Abstractions.Persistence;

public interface IDealsDbContext
{
    IQueryable<Deal> Deals { get; }

    Task AddDealAsync(Deal deal, CancellationToken ct);
    Task<Deal?> FindDealAsync(Guid dealId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
