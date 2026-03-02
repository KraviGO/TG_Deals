namespace Publishers.Entities.Channels;

public sealed record ChannelTitle
{
    public string Value { get; }

    public ChannelTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title is required.", nameof(value));
        if (value.Length > 200)
            throw new ArgumentException("Title too long (max 200).", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
