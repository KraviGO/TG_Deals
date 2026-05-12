using Microsoft.Extensions.DependencyInjection;
using Publishers.UseCases.Channels.CreateChannel;
using Publishers.UseCases.Channels.GetMyChannels;
using Publishers.UseCases.Channels.Moderation.BanChannel;
using Publishers.UseCases.Channels.Moderation.UnbanChannel;
using Publishers.UseCases.Channels.SetIntakeMode;
using Publishers.UseCases.Channels.UpdateChannel;
using Publishers.UseCases.Channels.Verification.ConfirmVerification;

namespace Publishers.Presentation.DependencyInjection;

public static class PublishersPresentationModule
{
    public static IServiceCollection AddPublishersPresentation(this IServiceCollection services)
    {
        services.AddScoped<CreateChannelHandler>();
        services.AddScoped<GetMyChannelsHandler>();
        services.AddScoped<UpdateChannelHandler>();
        services.AddScoped<SetIntakeModeHandler>();
        services.AddScoped<ConfirmVerificationHandler>();
        services.AddScoped<BanChannelHandler>();
        services.AddScoped<UnbanChannelHandler>();
        return services;
    }
}
