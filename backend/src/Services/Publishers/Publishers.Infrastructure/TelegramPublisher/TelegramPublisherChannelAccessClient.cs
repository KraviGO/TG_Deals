using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Marketplace.ServiceAuth.Options;
using Microsoft.Extensions.Options;
using Publishers.UseCases.Abstractions.Telegram;

namespace Publishers.Infrastructure.TelegramPublisher;

/// <summary>
/// Клиент telegram-publisher для проверки прав бота в канале.
/// </summary>
public sealed class TelegramPublisherChannelAccessClient : ITelegramChannelAccessClient
{
    private readonly HttpClient _http;
    private readonly string _serviceToken;

    public TelegramPublisherChannelAccessClient(
        HttpClient http,
        IOptions<TelegramPublisherClientOptions> opt,
        IOptions<ServiceAuthClientOptions> tokenOpt)
    {
        _http = http;
        _http.BaseAddress = new Uri(opt.Value.BaseUrl);
        _serviceToken = tokenOpt.Value.Token
            is { Length: > 0 } t
            ? t
            : throw new InvalidOperationException("ServiceAuth:Token missing for Publishers TelegramPublisher client.");
    }

    public async Task<ChannelBotAccess> CheckAccessAsync(string telegramChannelId, CancellationToken ct)
    {
        var encodedChannelId = Uri.EscapeDataString(telegramChannelId);
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/internal/telegram/channels/{encodedChannelId}");
        req.Headers.Add("X-Service-Token", _serviceToken);

        var res = await _http.SendAsync(req, ct);
        if (res.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            return new ChannelBotAccess(false, false, false, false);

        res.EnsureSuccessStatusCode();

        var dto = await res.Content.ReadFromJsonAsync<ChannelInfoDto>(cancellationToken: ct)
            ?? throw new InvalidOperationException("TelegramPublisher returned empty channel info response.");

        return new ChannelBotAccess(
            IsAccessible: true,
            IsBotAdmin: dto.IsBotAdmin,
            CanPostMessages: dto.CanPostMessages,
            CanDeleteMessages: dto.CanDeleteMessages);
    }

    private sealed record ChannelInfoDto(
        [property: JsonPropertyName("isBotAdmin")] bool IsBotAdmin,
        [property: JsonPropertyName("canPostMessages")] bool CanPostMessages,
        [property: JsonPropertyName("canDeleteMessages")] bool CanDeleteMessages);
}
