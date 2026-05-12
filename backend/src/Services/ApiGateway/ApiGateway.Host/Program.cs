using Marketplace.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseMarketplaceSerilog();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("GatewayCors", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddMarketplaceObservability();

// YARP берет маршруты из ReverseProxy config.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseMarketplaceObservability();
app.UseCors("GatewayCors");

app.MapReverseProxy();
app.MapGet("/", () => Results.Ok(new { service = "ApiGateway.Host", status = "ok" }));

app.Run();
