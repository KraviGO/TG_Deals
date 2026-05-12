namespace Payments.UseCases.Abstractions.YooKassa;

public sealed class YooKassaWebhookSecurityOptions
{
    public string[] AllowedIps { get; init; } = Array.Empty<string>();
    public bool RequireStatusVerification { get; init; } = true;
}
