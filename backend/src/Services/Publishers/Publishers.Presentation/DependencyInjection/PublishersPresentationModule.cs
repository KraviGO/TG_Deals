using Microsoft.Extensions.DependencyInjection;
using Publishers.UseCases.Channels.CreateChannel;
using Publishers.UseCases.Channels.GetMyChannels;
using Publishers.UseCases.Channels.SetIntakeMode;
using Publishers.UseCases.Channels.UpdateChannel;
using Publishers.UseCases.Channels.Verification.ConfirmVerification;
using Publishers.UseCases.Channels.Verification.StartVerification;

namespace Publishers.Presentation.DependencyInjection;

public static class PublishersPresentationModule
{
    public static IServiceCollection AddPublishersPresentation(this IServiceCollection services)
    {
        services.AddScoped<CreateChannelHandler>();
        services.AddScoped<GetMyChannelsHandler>();
        services.AddScoped<UpdateChannelHandler>();
        services.AddScoped<SetIntakeModeHandler>();
        services.AddScoped<StartVerificationHandler>();
        services.AddScoped<ConfirmVerificationHandler>();
        return services;
    }
}
