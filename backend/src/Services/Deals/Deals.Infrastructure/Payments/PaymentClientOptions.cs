namespace Deals.Infrastructure.Payments;

public sealed record PaymentClientOptions
{
    public string BaseUrl { get; init; } = default!;
}
