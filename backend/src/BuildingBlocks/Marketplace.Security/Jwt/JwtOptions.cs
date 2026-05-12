namespace Marketplace.Security.Jwt;

/// <summary>
/// Настройки выпуска и проверки JWT.
/// Значения читаются из конфигурационной секции Jwt.
/// </summary>
public sealed record JwtOptions
{
    /// <summary>
    /// Издатель токена.
    /// </summary>
    public string Issuer { get; init; } = default!;

    /// <summary>
    /// Получатель токена.
    /// </summary>
    public string Audience { get; init; } = default!;

    /// <summary>
    /// Симметричный ключ подписи JWT.
    /// </summary>
    public string SigningKey { get; init; } = default!;

    /// <summary>
    /// Срок жизни access token в минутах.
    /// </summary>
    public int ExpiresMinutes { get; init; } = 60;
}
