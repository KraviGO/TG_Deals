namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed record SearchChannelsQuery(int Limit = 50, int Offset = 0);
