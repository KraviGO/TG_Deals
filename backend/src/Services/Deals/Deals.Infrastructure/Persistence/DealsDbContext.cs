using Deals.Entities.Deals;
using Deals.Entities.Disputes;
using Deals.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Deals.Infrastructure.Persistence;

public sealed class DealsDbContext : DbContext, IDealsDbContext
{
    public DealsDbContext(DbContextOptions<DealsDbContext> options) : base(options) { }

    public DbSet<Deal> DealsSet => Set<Deal>();
    public IQueryable<Deal> Deals => DealsSet.AsQueryable();
    public DbSet<DealDispute> DisputesSet => Set<DealDispute>();
    public IQueryable<DealDispute> Disputes => DisputesSet.AsQueryable();

    public Task AddDealAsync(Deal deal, CancellationToken ct) => DealsSet.AddAsync(deal, ct).AsTask();
    public Task AddDisputeAsync(DealDispute dispute, CancellationToken ct) => DisputesSet.AddAsync(dispute, ct).AsTask();

    public Task<Deal?> FindDealAsync(Guid dealId, CancellationToken ct)
        => DealsSet.FirstOrDefaultAsync(x => x.DealId == dealId, ct);

    Task<int> IDealsDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DealsDbContext).Assembly);
    }
}
