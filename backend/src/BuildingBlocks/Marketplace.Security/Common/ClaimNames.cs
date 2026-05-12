namespace Marketplace.Security.Common;

/// <summary>
/// Контрактные имена JWT claims.
/// Эти константы используют генерация токена и authorization policies.
/// </summary>
public static class ClaimNames
{
    /// <summary>
    /// Subject пользователя. Сервисы используют его как идентификатор пользователя.
    /// </summary>
    public const string Subject = "sub";

    /// <summary>
    /// Роль пользователя в маркетплейсе.
    /// </summary>
    public const string Role = "role";

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public const string Email = "email";
}
