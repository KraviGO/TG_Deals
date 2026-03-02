using Marketplace.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Infrastructure.Clock;
using Payments.Infrastructure.Outbox;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.YooKassa;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Messaging;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Abstractions.YooKassa;

namespace Payments.Infrastructure.DependencyInjection;

public static class PaymentsInfrastructureModule
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<PaymentsDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Database")));

        services.AddScoped<IPaymentsDbContext>(sp => sp.GetRequiredService<PaymentsDbContext>());
        services.AddSingleton<IClock, SystemClock>();

        services.Configure<RabbitMqOptions>(cfg.GetSection("RabbitMq"));
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddHostedService<OutboxPublisherWorker>();

        services.Configure<YooKassaOptions>(cfg.GetSection("YooKassa"));
        services.AddHttpClient<IYooKassaClient, YooKassaClient>();

        return services;
    }
}
