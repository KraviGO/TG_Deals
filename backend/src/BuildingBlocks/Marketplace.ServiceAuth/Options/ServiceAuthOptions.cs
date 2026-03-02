namespace Marketplace.ServiceAuth.Options;

public sealed record ServiceAuthOptions
{
    public string Token { get; init; } = default!;
    public string PathPrefix { get; init; } = "/api/v1/internal";
}
