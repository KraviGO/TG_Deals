using Marketplace.Messaging.RabbitMq;
using Marketplace.ServiceAuth.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publishers.Infrastructure.Clock;
using Publishers.Infrastructure.Outbox;
using Publishers.Infrastructure.Persistence;
using Publishers.Infrastructure.TelegramPublisher;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Abstractions.Telegram;

namespace Publishers.Infrastructure.DependencyInjection;

public static class PublishersInfrastructureModule
{
    public static IServiceCollection AddPublishersInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<PublishersDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Database")));

        services.AddScoped<IPublishersDbContext>(sp => sp.GetRequiredService<PublishersDbContext>());
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();

        services.Configure<TelegramPublisherClientOptions>(cfg.GetSection("TelegramPublisherClient"));
        services.Configure<ServiceAuthClientOptions>(cfg.GetSection(ServiceAuthClientOptions.SectionName));

        services.AddHttpClient<ITelegramChannelAccessClient, TelegramPublisherChannelAccessClient>()
            .AddHttpMessageHandler<Marketplace.Observability.Http.CorrelationIdDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.Configure<RabbitMqOptions>(cfg.GetSection("RabbitMq"));
        services.AddHostedService<OutboxPublisherWorker>();

        return services;
    }
}
