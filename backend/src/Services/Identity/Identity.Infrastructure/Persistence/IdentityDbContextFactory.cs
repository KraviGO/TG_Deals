using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var hostPath = Path.Combine(basePath, "Identity.Host");
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

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
