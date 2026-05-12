using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Marketplace.Observability.Http;

public sealed class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is not null)
        {
            if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues correlationIds))
            {
                var correlationId = correlationIds.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(correlationId) && !request.Headers.Contains(CorrelationIdHeaderName))
                {
                    request.Headers.Add(CorrelationIdHeaderName, correlationId);
                }
            }
            else if (!request.Headers.Contains(CorrelationIdHeaderName))
            {
                // Исходящий запрос получает trace identifier текущего HTTP-запроса.
                request.Headers.Add(CorrelationIdHeaderName, httpContext.TraceIdentifier);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
