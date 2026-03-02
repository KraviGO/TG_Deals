using Payments.UseCases.Abstractions.Clock;

namespace Payments.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
