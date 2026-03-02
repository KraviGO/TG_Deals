namespace Deals.Infrastructure.Catalog;

public sealed record CatalogClientOptions
{
    public string BaseUrl { get; init; } = default!;
}
