using ChannelCatalog.UseCases.Channels.GetChannelById;
using ChannelCatalog.UseCases.Channels.GetInternalChannelById;
using ChannelCatalog.UseCases.Channels.SearchChannels;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelCatalog.Presentation.DependencyInjection;

public static class ChannelCatalogPresentationModule
{
    public static IServiceCollection AddChannelCatalogPresentation(this IServiceCollection services)
    {
        services.AddScoped<SearchChannelsHandler>();
        services.AddScoped<GetChannelByIdHandler>();
        services.AddScoped<GetInternalChannelByIdHandler>();
        return services;
    }
}
