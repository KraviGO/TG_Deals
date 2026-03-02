using System.Text;
using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Security.Jwt;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddMarketplaceJwt(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

        var jwt = cfg.GetSection("Jwt").Get<JwtOptions>()
                  ?? throw new InvalidOperationException("Jwt options are missing in configuration.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                // Важно: чтобы sub/role не переписывались в странные URI-клеймы
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),

                    NameClaimType = ClaimNames.Subject,
                    RoleClaimType = ClaimNames.Role,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        return services;
    }
}
