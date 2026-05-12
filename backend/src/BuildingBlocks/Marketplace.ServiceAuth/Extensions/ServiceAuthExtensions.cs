using Marketplace.ServiceAuth.Authentication;
using Marketplace.ServiceAuth.Constants;
using Marketplace.ServiceAuth.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.ServiceAuth.Extensions;

/// <summary>
/// Методы расширения для service-to-service авторизации.
/// Авторизация проверяет shared token из конфигурации.
/// </summary>
public static class ServiceAuthExtensions
{
    /// <summary>
    /// Регистрирует настройки ServiceAuth и authentication scheme ServiceToken.
    /// </summary>
    /// <param name="services">DI container приложения.</param>
    /// <param name="cfg">Конфигурация приложения.</param>
    /// <returns>Та же коллекция сервисов для продолжения настройки.</returns>
    public static IServiceCollection AddServiceAuth(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<ServiceAuthOptions>(ServiceAuthDefaults.AuthenticationScheme, cfg.GetSection("ServiceAuth"));
        
        services.AddAuthentication()
            .AddScheme<ServiceAuthOptions, ServiceTokenAuthenticationHandler>(ServiceAuthDefaults.AuthenticationScheme, options =>
            {
                // Настройки схемы загружены через Configure выше.
            });

        return services;
    }
}
