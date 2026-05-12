namespace Payments.UseCases.Abstractions.Fees;

public sealed class PlatformFeesOptions
{
    public int PlatformFeeBps { get; set; } = 1000;
}
