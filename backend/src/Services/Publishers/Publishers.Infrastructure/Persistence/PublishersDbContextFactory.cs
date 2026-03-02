using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Publishers.Infrastructure.Persistence;

public sealed class PublishersDbContextFactory : IDesignTimeDbContextFactory<PublishersDbContext>
{
    public PublishersDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var hostPath = Path.Combine(basePath, "Publishers.Host");
        var cfgBuilder = new ConfigurationBuilder();

        if (File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            cfgBuilder.SetBasePath(basePath);
        }
        else if (Directory.Exists(hostPath))
        {
            cfgBuilder.SetBasePath(hostPath);
        }
        else
        {
            cfgBuilder.SetBasePath(basePath);
        }

        cfgBuilder
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables();

        var configuration = cfgBuilder.Build();
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PublishersDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PublishersDbContext(optionsBuilder.Options);
    }
}
