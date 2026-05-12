using System.Net.Http.Json;
using Deals.UseCases.Abstractions.Wallet;
using Marketplace.ServiceAuth.Options;
using Microsoft.Extensions.Options;

namespace Deals.Infrastructure.Wallet;

public sealed class WalletClient : IWalletClient
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public WalletClient(HttpClient http, IOptions<WalletClientOptions> opt, IOptions<ServiceAuthClientOptions> tokenOpt)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);

        _serviceToken = tokenOpt.Value.Token
            is { Length: > 0 } t
            ? t
            : throw new InvalidOperationException("ServiceAuth:Token missing for Deals WalletClient.");
    }

    public async Task<WalletReservationResult> ReserveForDealAsync(
        Guid dealId, Guid advertiserUserId, Guid publisherUserId, decimal amount, string currency, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal/wallet/reservations");
        req.Headers.Add("X-Service-Token", _serviceToken);
        req.Content = JsonContent.Create(new
        {
            dealId,
            advertiserUserId,
            publisherUserId,
            amount,
            currency
        });

        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var dto = await res.Content.ReadFromJsonAsync<WalletReservationResponseDto>(cancellationToken: ct);
        if (dto is null) throw new InvalidOperationException("Failed to parse reservation response.");

        return new WalletReservationResult(dto.ReservationId, dto.Status);
    }

    public async Task ReleaseReservationAsync(Guid dealId, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/internal/wallet/reservations/{dealId}/release");
        req.Headers.Add("X-Service-Token", _serviceToken);
        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task CaptureReservationAsync(Guid dealId, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/internal/wallet/reservations/{dealId}/capture");
        req.Headers.Add("X-Service-Token", _serviceToken);
        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
    }

    private sealed record WalletReservationResponseDto(Guid ReservationId, string Status);
}
