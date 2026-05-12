using System.Security.Claims;

namespace Marketplace.Security.Common;

/// <summary>
/// Методы расширения для чтения claims текущего пользователя.
/// Контроллеры используют эти методы вместо прямого поиска claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Возвращает идентификатор пользователя из sub или NameIdentifier claim.
    /// Возвращает Guid.Empty, если claim отсутствует или не содержит Guid.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub")
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    /// Возвращает роль пользователя из role claim.
    /// </summary>
    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue("role") ?? string.Empty;
}
