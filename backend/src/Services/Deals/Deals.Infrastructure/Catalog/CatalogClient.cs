using System.Net.Http.Json;
using Deals.UseCases.Abstractions.Catalog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Deals.Infrastructure.Catalog;

public sealed class CatalogClient : ICatalogClient
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public CatalogClient(HttpClient http, IOptions<CatalogClientOptions> opt, IConfiguration cfg)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);

        _serviceToken = cfg.GetSection("ServiceAuth")["Token"]
                        ?? throw new InvalidOperationException("ServiceAuth:Token missing for Deals.");
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
