using Identity.Entities.Common;

namespace Identity.Entities.Users;

/// <summary>
/// Пользователь marketplace с ролью и статусом доступа.
/// </summary>
public sealed class User : Entity
{
    private User() { }

    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Создает активного пользователя с нормализованным email.
    /// </summary>
    public static User Create(string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Блокирует вход пользователя.
    /// </summary>
    public void Suspend() => Status = UserStatus.Suspended;
}
