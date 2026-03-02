using Deals.UseCases.Abstractions.Clock;

namespace Deals.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
