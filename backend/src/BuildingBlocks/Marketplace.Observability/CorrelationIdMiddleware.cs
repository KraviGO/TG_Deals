using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Marketplace.Observability;

/// <summary>
/// Middleware назначает correlation id для HTTP-запроса.
/// Значение берется из входящего заголовка, Activity trace id или ASP.NET trace identifier.
/// Middleware добавляет correlation id в response header и log scope.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    /// <summary>
    /// HTTP-заголовок для передачи correlation id между сервисами.
    /// </summary>
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _log;

    /// <summary>
    /// Создает middleware.
    /// </summary>
    /// <param name="next">Следующий компонент HTTP pipeline.</param>
    /// <param name="log">Логгер для scope с correlation id.</param>
    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    /// <summary>
    /// Обрабатывает запрос и продолжает pipeline.
    /// </summary>
    /// <param name="context">Текущий HTTP-контекст.</param>
    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing)
            ? existing.ToString()
            : string.Empty;

        // Запрос без входящего correlation id получает trace id текущей Activity.
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (_log.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }
}
