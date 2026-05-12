using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;
using System.Threading.RateLimiting;

namespace Marketplace.Observability;

/// <summary>
/// Методы расширения для логов, correlation id, HTTP-метрик Prometheus и rate limiting.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Подключает Serilog как logging provider приложения.
    /// Настройки читаются из host configuration и дополняются console sink.
    /// </summary>
    /// <param name="host">Host builder ASP.NET приложения.</param>
    /// <returns>Тот же host builder.</returns>
    public static IHostBuilder UseMarketplaceSerilog(this IHostBuilder host)
    {
        return host.UseSerilog((ctx, services, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
               .ReadFrom.Services(services)
               .Enrich.FromLogContext()
               .WriteTo.Console();
        });
    }

    /// <summary>
    /// Регистрирует сервисы для middleware наблюдаемости.
    /// </summary>
    /// <param name="services">DI container приложения.</param>
    /// <returns>Та же коллекция сервисов для продолжения настройки.</returns>
    public static IServiceCollection AddMarketplaceObservability(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<Marketplace.Observability.Http.CorrelationIdDelegatingHandler>();
        return services;
    }

    /// <summary>
    /// Добавляет глобальный fixed-window rate limiter по IP-адресу клиента.
    /// Значение по умолчанию: 120 запросов в минуту без очереди ожидания.
    /// </summary>
    /// <param name="services">DI container приложения.</param>
    /// <param name="permitLimit">Лимит запросов за одну минуту.</param>
    /// <returns>Та же коллекция сервисов для продолжения настройки.</returns>
    public static IServiceCollection AddMarketplaceRateLimiting(this IServiceCollection services, int permitLimit = 120)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ip,
                    // Каждый IP-адрес получает отдельный limiter.
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }

    /// <summary>
    /// Подключает middleware наблюдаемости в HTTP pipeline.
    /// Pipeline добавляет correlation id, request logging, HTTP metrics и endpoint /metrics.
    /// </summary>
    /// <param name="app">Application builder ASP.NET приложения.</param>
    /// <returns>Тот же application builder.</returns>
    public static IApplicationBuilder UseMarketplaceObservability(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging();
        app.UseHttpMetrics();
        app.UseMetricServer("/metrics");
        return app;
    }
}
