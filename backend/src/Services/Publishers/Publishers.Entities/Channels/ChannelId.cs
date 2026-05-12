namespace Publishers.Entities.Channels;

/// <summary>
/// Идентификатор канала внутри marketplace.
/// </summary>
public readonly record struct ChannelId(Guid Value)
{
    public static ChannelId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
