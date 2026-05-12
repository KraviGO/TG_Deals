using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Deals.UseCases.Abstractions.Telegram;
using Marketplace.ServiceAuth.Options;
using Microsoft.Extensions.Options;

namespace Deals.Infrastructure.TelegramPublisher;

public sealed class TelegramPostPublisher : ITelegramPostPublisher
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public TelegramPostPublisher(
        HttpClient http,
        IOptions<TelegramPublisherClientOptions> opt,
        IOptions<ServiceAuthClientOptions> tokenOpt)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);
        _serviceToken = tokenOpt.Value.Token
            is { Length: > 0 } t
            ? t
            : throw new InvalidOperationException("ServiceAuth:Token missing for Deals TelegramPublisher client.");
    }

    public async Task<TelegramPostPublishResult> PublishTextAsync(string telegramChannelId, string text, CancellationToken ct)
    {
        var encodedChannelId = Uri.EscapeDataString(telegramChannelId);
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/internal/telegram/channels/{encodedChannelId}/posts");
        req.Headers.Add("X-Service-Token", _serviceToken);
        req.Content = JsonContent.Create(new PublishTextRequest(text));

        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var dto = await res.Content.ReadFromJsonAsync<PublishPostResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("TelegramPublisher returned empty publish response.");

        return new TelegramPostPublishResult(dto.MessageId, dto.PostedAtUtc, dto.PostUrl);
    }

    private sealed record PublishTextRequest(
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("disableWebPagePreview")] bool DisableWebPagePreview = false);

    private sealed record PublishPostResponse(
        [property: JsonPropertyName("messageId")] int MessageId,
        [property: JsonPropertyName("postedAtUtc")] DateTimeOffset PostedAtUtc,
        [property: JsonPropertyName("postUrl")] string? PostUrl);
}
