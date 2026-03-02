using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Payments.UseCases.Abstractions.YooKassa;

namespace Payments.Infrastructure.YooKassa;

public sealed class YooKassaClient : IYooKassaClient
{
    private readonly HttpClient _http;
    private readonly YooKassaOptions _opt;

    public YooKassaClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _opt = cfg.GetSection("YooKassa").Get<YooKassaOptions>()
               ?? throw new InvalidOperationException("YooKassa options missing");

        _http.BaseAddress = new Uri("https://api.yookassa.ru/");
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_opt.ShopId}:{_opt.SecretKey}"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
    }

    public async Task<YooKassaCreatePaymentResult> CreateTwoStagePaymentAsync(
        YooKassaCreatePaymentRequest req,
        Guid idempotenceKey,
        CancellationToken ct)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/v3/payments");
        msg.Headers.Add("Idempotence-Key", idempotenceKey.ToString());

        var body = new
        {
            amount = new { value = req.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), currency = req.Currency },
            capture = false,
            confirmation = new { type = "redirect", return_url = req.ReturnUrl },
            description = req.Description
        };

        msg.Content = JsonContent.Create(body);

        var res = await _http.SendAsync(msg, ct);
        res.EnsureSuccessStatusCode();

        await using var stream = await res.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var root = doc.RootElement;
        var id = root.GetProperty("id").GetString() ?? throw new InvalidOperationException("YooKassa response missing id");
        var status = root.TryGetProperty("status", out var statusEl) ? statusEl.GetString() ?? "" : "";

        string? confirmationUrl = null;
        if (root.TryGetProperty("confirmation", out var conf) &&
            conf.ValueKind == JsonValueKind.Object &&
            conf.TryGetProperty("confirmation_url", out var urlEl))
        {
            confirmationUrl = urlEl.GetString();
        }

        return new YooKassaCreatePaymentResult(id, status, confirmationUrl);
    }

    public async Task CaptureAsync(string yooKassaPaymentId, Guid idempotenceKey, CancellationToken ct)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/v3/payments/{yooKassaPaymentId}/capture");
        msg.Headers.Add("Idempotence-Key", idempotenceKey.ToString());
        msg.Content = JsonContent.Create(new { });

        var res = await _http.SendAsync(msg, ct);
        res.EnsureSuccessStatusCode();
    }
}
