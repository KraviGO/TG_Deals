namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed record SearchChannelsQuery(
    int Limit = 50,
    int Offset = 0,
    string? Search = null,
    string? Topic = null,
    string? Language = null,
    string? IntakeMode = null,
    decimal? MinPricePerPostRub = null,
    decimal? MaxPricePerPostRub = null,
    string? SortBy = null,
    string? SortOrder = null);
