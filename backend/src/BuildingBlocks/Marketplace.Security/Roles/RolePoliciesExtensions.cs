using Marketplace.Security.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Security.Roles;

public static class RolePoliciesExtensions
{
    public static IServiceCollection AddMarketplaceRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Advertiser", p => p.RequireClaim(ClaimNames.Role, "Advertiser"));
            options.AddPolicy("Publisher", p => p.RequireClaim(ClaimNames.Role, "Publisher"));
            options.AddPolicy("Admin", p => p.RequireClaim(ClaimNames.Role, "Admin"));
        });

        return services;
    }
}
