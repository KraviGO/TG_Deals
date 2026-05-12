using System.Net.Http.Json;
using Deals.UseCases.Abstractions.Catalog;
using Marketplace.ServiceAuth.Options;
using Microsoft.Extensions.Options;

namespace Deals.Infrastructure.Catalog;

public sealed class CatalogClient : ICatalogClient
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public CatalogClient(HttpClient http, IOptions<CatalogClientOptions> opt, IOptions<ServiceAuthClientOptions> tokenOpt)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);

        _serviceToken = tokenOpt.Value.Token
            is { Length: > 0 } t
            ? t
            : throw new InvalidOperationException("ServiceAuth:Token missing for Deals CatalogClient.");
    }

    public async Task<CatalogChannelInfo?> GetChannelAsync(Guid channelId, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/internal/channels/{channelId}");
        req.Headers.Add("X-Service-Token", _serviceToken);

        var res = await _http.SendAsync(req, ct);
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();

        var dto = await res.Content.ReadFromJsonAsync<CatalogChannelInfo>(cancellationToken: ct);
        return dto;
    }
}
