using Deals.Infrastructure.Catalog;
using Deals.Infrastructure.Clock;
using Deals.Infrastructure.Consumers;
using Deals.Infrastructure.Consumers.Payments;
using Deals.Infrastructure.Payments;
using Deals.Infrastructure.Persistence;
using Deals.UseCases.Abstractions.Catalog;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Payments;
using Deals.UseCases.Abstractions.Persistence;
using Marketplace.Messaging.RabbitMq;
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
        services.Configure<PaymentClientOptions>(cfg.GetSection("PaymentClient"));
        services.Configure<RabbitMqOptions>(cfg.GetSection("RabbitMq"));

        services.AddHttpClient<ICatalogClient, CatalogClient>();
        services.AddHttpClient<IPaymentsClient, PaymentClient>();

        services.AddScoped<IEventHandler, PaymentAuthorizedHandler>();
        services.AddScoped<IEventHandler, PaymentCapturedHandler>();
        services.AddScoped<IEventHandler, PaymentCanceledHandler>();
        services.AddHostedService<PaymentsEventsConsumer>();

        return services;
    }
}
