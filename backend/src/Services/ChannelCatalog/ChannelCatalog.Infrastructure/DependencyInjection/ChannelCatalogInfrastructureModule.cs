using ChannelCatalog.Infrastructure.Consumers;
using ChannelCatalog.Infrastructure.Consumers.Publishers;
using ChannelCatalog.Infrastructure.Persistence;
using ChannelCatalog.UseCases.Abstractions.Persistence;
using Marketplace.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelCatalog.Infrastructure.DependencyInjection;

public static class ChannelCatalogInfrastructureModule
{
    public static IServiceCollection AddChannelCatalogInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<CatalogDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Database")));

        services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.Configure<RabbitMqOptions>(cfg.GetSection("RabbitMq"));

        services.AddScoped<IEventHandler, ChannelRegisteredHandler>();
        services.AddScoped<IEventHandler, ChannelOwnershipVerifiedHandler>();

        services.AddHostedService<PublishersEventsConsumer>();

        return services;
    }
}
