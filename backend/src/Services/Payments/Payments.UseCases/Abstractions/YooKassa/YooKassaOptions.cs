namespace Payments.UseCases.Abstractions.YooKassa;

public sealed record YooKassaOptions
{
    public string ShopId { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public string ReturnUrl { get; init; } = default!;
    public bool TestMode { get; init; } = true;
}
