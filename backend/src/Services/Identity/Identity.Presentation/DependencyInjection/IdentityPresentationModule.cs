using Identity.UseCases.Auth.Login;
using Identity.UseCases.Auth.Me;
using Identity.UseCases.Auth.Register;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Presentation.DependencyInjection;

public static class IdentityPresentationModule
{
    public static IServiceCollection AddIdentityPresentation(this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<MeHandler>();
        return services;
    }
}
