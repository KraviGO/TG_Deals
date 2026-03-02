using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Deals.Infrastructure.Persistence;

public sealed class DealsDbContextFactory : IDesignTimeDbContextFactory<DealsDbContext>
{
    public DealsDbContext CreateDbContext(string[] args)
    {
        var opt = new DbContextOptionsBuilder<DealsDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=deals_db;Username=marketplace;Password=marketplace")
            .Options;

        return new DealsDbContext(opt);
    }
}
