namespace Publishers.Entities.Channels;

/// <summary>
/// Статус канала в контуре владения и модерации.
/// </summary>
public enum OwnershipStatus
{
    /// <summary>Ожидает проверки прав Telegram-бота.</summary>
    PendingVerification = 1,

    /// <summary>Права Telegram-бота подтверждены.</summary>
    Verified = 2,

    /// <summary>Канал отклонен.</summary>
    Rejected = 3,

    /// <summary>Канал забанен.</summary>
    Banned = 4
}
