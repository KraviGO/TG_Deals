namespace Payments.UseCases.Abstractions.Clock;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
