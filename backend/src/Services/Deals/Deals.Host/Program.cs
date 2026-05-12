using Deals.Infrastructure.DependencyInjection;
using Deals.Presentation.DependencyInjection;
using Marketplace.Observability;
using Marketplace.Security.Jwt;
using Marketplace.Security.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Deals.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseMarketplaceSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Deals API", Version = "v1" });

    // Swagger UI отправляет JWT в закрытые endpoint'ы.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMarketplaceJwt(builder.Configuration);
builder.Services.AddMarketplaceRolePolicies();
builder.Services.AddMarketplaceObservability();
builder.Services.AddMarketplaceRateLimiting(builder.Configuration.GetValue<int?>("RateLimiting:PermitLimit") ?? 120);

builder.Services.AddDealsInfrastructure(builder.Configuration);
builder.Services.AddDealsPresentation();

builder.Services.AddHealthChecks();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    is { Length: > 0 } origins ? origins : new[] { "http://localhost:3000" };

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Frontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

var autoMigrate = app.Configuration.GetValue<bool?>("Database:AutoMigrate") ?? app.Environment.IsDevelopment();
if (autoMigrate)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DealsDbContext>();
    db.Database.Migrate();
}

app.UseMarketplaceObservability();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health").AllowAnonymous();
app.MapHealthChecks("/ready").AllowAnonymous();

app.UseRateLimiter();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { service = "Deals.Host", status = "ok" })).AllowAnonymous();

app.Run();
