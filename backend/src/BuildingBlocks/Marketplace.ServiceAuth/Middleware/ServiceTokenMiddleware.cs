using System.Net;
using Marketplace.ServiceAuth.Constants;
using Marketplace.ServiceAuth.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Marketplace.ServiceAuth.Middleware;

public sealed class ServiceTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ServiceAuthOptions _opt;

    public ServiceTokenMiddleware(RequestDelegate next, IOptions<ServiceAuthOptions> options)
    {
        _next = next;
        _opt = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        // Проверяем только internal prefix
        if (!context.Request.Path.StartsWithSegments(_opt.PathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Достаём заголовок
        if (!context.Request.Headers.TryGetValue(ServiceAuthHeaderNames.ServiceToken, out var tokenValues))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "ServiceTokenMissing" });
            return;
        }

        var token = tokenValues.ToString();

        // Сравнение в постоянное время (минимальная защита от timing attacks)
        if (!FixedTimeEquals(token, _opt.Token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "ServiceTokenInvalid" });
            return;
        }

        // Помечаем запрос как internal/service-authenticated (на будущее)
        context.Items["IsServiceAuthenticated"] = true;

        await _next(context);
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a is null || b is null) return false;
        if (a.Length != b.Length) return false;

        var diff = 0;
        for (var i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];

        return diff == 0;
    }
}
