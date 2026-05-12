using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Deals.Infrastructure.Persistence;

public sealed class DealsDbContextFactory : IDesignTimeDbContextFactory<DealsDbContext>
{
    public DealsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? "Host=localhost;Port=5432;Database=deals_db;Username=marketplace;Password=marketplace";

        var opt = new DbContextOptionsBuilder<DealsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DealsDbContext(opt);
    }
}
