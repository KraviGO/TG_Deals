namespace Marketplace.Security.Jwt;

public sealed record JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SigningKey { get; init; } = default!;
    public int ExpiresMinutes { get; init; } = 60;
}
