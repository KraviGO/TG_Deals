using Deals.Infrastructure.Catalog;
using Deals.Infrastructure.Clock;
using Deals.Infrastructure.TelegramPublisher;
using Deals.Infrastructure.Wallet;
using Deals.Infrastructure.Persistence;
using Deals.UseCases.Abstractions.Catalog;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Abstractions.Telegram;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.ServiceAuth.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deals.Infrastructure.DependencyInjection;

public static class DealsInfrastructureModule
{
    public static IServiceCollection AddDealsInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<DealsDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Database")));

        services.AddScoped<IDealsDbContext>(sp => sp.GetRequiredService<DealsDbContext>());
        services.AddSingleton<IClock, SystemClock>();

        services.Configure<CatalogClientOptions>(cfg.GetSection("CatalogClient"));
        services.Configure<WalletClientOptions>(cfg.GetSection("WalletClient"));
        services.Configure<TelegramPublisherClientOptions>(cfg.GetSection("TelegramPublisherClient"));
        services.Configure<ServiceAuthClientOptions>(cfg.GetSection(ServiceAuthClientOptions.SectionName));

        services.AddHttpClient<ICatalogClient, CatalogClient>()
            .AddHttpMessageHandler<Marketplace.Observability.Http.CorrelationIdDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.AddHttpClient<IWalletClient, WalletClient>()
            .AddHttpMessageHandler<Marketplace.Observability.Http.CorrelationIdDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.AddHttpClient<ITelegramPostPublisher, TelegramPostPublisher>()
            .AddHttpMessageHandler<Marketplace.Observability.Http.CorrelationIdDelegatingHandler>()
            .AddStandardResilienceHandler();

        return services;
    }
}
