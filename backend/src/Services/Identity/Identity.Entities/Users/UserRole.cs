namespace Identity.Entities.Users;

/// <summary>
/// Роль пользователя в marketplace.
/// </summary>
public enum UserRole
{
    /// <summary>Рекламодатель.</summary>
    Advertiser = 1,

    /// <summary>Паблишер.</summary>
    Publisher = 2,

    /// <summary>Администратор.</summary>
    Admin = 3
}
