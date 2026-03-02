using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Persistence;
using Identity.UseCases.Abstractions.Auth;
using Identity.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.DependencyInjection;

public static class IdentityInfrastructureModule
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

        services.AddDbContext<IdentityDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Database")));

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();

        return services;
    }
}
