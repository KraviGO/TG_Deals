using Marketplace.Security.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Security.Roles;

/// <summary>
/// Методы расширения для регистрации role-based authorization policies.
/// Политики проверяют role claim из JWT.
/// </summary>
public static class RolePoliciesExtensions
{
    /// <summary>
    /// Добавляет политики Advertiser, Publisher и Admin.
    /// Policies используются через AuthorizeAttribute или endpoint metadata.
    /// </summary>
    /// <param name="services">DI container приложения.</param>
    /// <returns>Та же коллекция сервисов для продолжения настройки.</returns>
    public static IServiceCollection AddMarketplaceRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Каждая policy требует конкретное значение role claim.
            options.AddPolicy("Advertiser", p => p.RequireClaim(ClaimNames.Role, "Advertiser"));
            options.AddPolicy("Publisher", p => p.RequireClaim(ClaimNames.Role, "Publisher"));
            options.AddPolicy("Admin", p => p.RequireClaim(ClaimNames.Role, "Admin"));

            // Endpoint без AllowAnonymous требует аутентифицированного пользователя.
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
