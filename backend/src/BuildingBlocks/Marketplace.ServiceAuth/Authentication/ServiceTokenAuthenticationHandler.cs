using System.Security.Claims;
using System.Text.Encodings.Web;
using Marketplace.ServiceAuth.Constants;
using Marketplace.ServiceAuth.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.ServiceAuth.Authentication;

/// <summary>
/// Authentication handler для внутренних HTTP-вызовов между сервисами.
/// Handler проверяет общий секрет из заголовка X-Service-Token.
/// </summary>
public class ServiceTokenAuthenticationHandler : AuthenticationHandler<ServiceAuthOptions>
{
    public ServiceTokenAuthenticationHandler(
        IOptionsMonitor<ServiceAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Запрос без X-Service-Token передается другим authentication-схемам.
        if (!Request.Headers.TryGetValue(ServiceAuthHeaderNames.ServiceToken, out var tokenValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = tokenValues.ToString();

        if (!FixedTimeEquals(token, Options.Token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Service Token"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "Service")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a is null || b is null) return false;
        if (a.Length != b.Length) return false;

        // Сравнение выполняет одинаковый цикл для строк равной длины.
        var diff = 0;
        for (var i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];

        return diff == 0;
    }
}
