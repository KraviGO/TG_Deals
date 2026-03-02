using System.Net.Http.Json;
using Deals.UseCases.Abstractions.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Deals.Infrastructure.Payments;

public sealed class PaymentClient : IPaymentsClient
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public PaymentClient(HttpClient http, IOptions<PaymentClientOptions> opt, IConfiguration cfg)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);

        _serviceToken = cfg.GetSection("ServiceAuth")["Token"]
                        ?? throw new InvalidOperationException("ServiceAuth:Token missing for Deals.");
    }

    public async Task<PaymentCreateResult> CreatePaymentAsync(Guid dealId, Guid advertiserUserId, decimal amount, string currency, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal/payments");
        req.Headers.Add("X-Service-Token", _serviceToken);
        req.Content = JsonContent.Create(new
        {
            dealId,
            advertiserUserId,
            amount,
            currency
        });

        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var dto = await res.Content.ReadFromJsonAsync<PaymentCreateResponseDto>(cancellationToken: ct);
        if (dto is null) throw new InvalidOperationException("Failed to parse payment create response.");

        return new PaymentCreateResult(dto.PaymentId, dto.ConfirmationUrl);
    }

    private sealed record PaymentCreateResponseDto(Guid PaymentId, string ConfirmationUrl);
}
