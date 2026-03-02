using Marketplace.ServiceAuth.Middleware;
using Marketplace.ServiceAuth.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.ServiceAuth.Extensions;

public static class ServiceAuthExtensions
{
    public static IServiceCollection AddServiceAuth(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<ServiceAuthOptions>(cfg.GetSection("ServiceAuth"));
        return services;
    }

    public static IApplicationBuilder UseServiceAuth(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ServiceTokenMiddleware>();
    }
}
