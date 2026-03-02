using Publishers.UseCases.Abstractions.Clock;

namespace Publishers.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
