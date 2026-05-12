using System.Text;
using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Security.Jwt;

/// <summary>
/// Методы расширения для JWT Bearer-аутентификации.
/// Настройка задает issuer, audience, signing key и имена claims.
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Регистрирует JWT Bearer authentication и проверку токена по секции Jwt.
    /// </summary>
    /// <param name="services">DI container приложения.</param>
    /// <param name="cfg">Конфигурация приложения с секцией Jwt.</param>
    /// <returns>Та же коллекция сервисов для продолжения настройки.</returns>
    /// <exception cref="InvalidOperationException">Выбрасывается, если секция Jwt отсутствует или чтение секции завершилось ошибкой.</exception>
    public static IServiceCollection AddMarketplaceJwt(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

        var jwt = cfg.GetSection("Jwt").Get<JwtOptions>()
                  ?? throw new InvalidOperationException("Jwt options are missing in configuration.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Metadata endpoint доступен без HTTPS в локальном и внутреннем окружении.
                options.RequireHttpsMetadata = false;

                // Сервисы читают короткие claim names без преобразования в URI.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new()
                {
                    // Проверка отклоняет токены с другим issuer, audience, сроком жизни или подписью.
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),

                    NameClaimType = ClaimNames.Subject,
                    RoleClaimType = ClaimNames.Role,
                    // Допуск 30 секунд покрывает рассинхронизацию часов между сервисами.
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        return services;
    }
}
