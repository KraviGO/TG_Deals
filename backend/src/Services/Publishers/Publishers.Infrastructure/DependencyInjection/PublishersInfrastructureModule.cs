using Marketplace.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publishers.Infrastructure.Clock;
using Publishers.Infrastructure.Outbox;
using Publishers.Infrastructure.Persistence;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;

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

        services.Configure<RabbitMqOptions>(cfg.GetSection("RabbitMq"));
        services.AddHostedService<OutboxPublisherWorker>();

        return services;
    }
}
