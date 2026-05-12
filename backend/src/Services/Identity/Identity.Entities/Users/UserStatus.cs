namespace Identity.Entities.Users;

/// <summary>
/// Статус доступа пользователя.
/// </summary>
public enum UserStatus
{
    /// <summary>Вход в систему разрешен.</summary>
    Active = 1,

    /// <summary>Вход пользователя заблокирован.</summary>
    Suspended = 2,

    /// <summary>Пользователь удален.</summary>
    Deleted = 3
}
