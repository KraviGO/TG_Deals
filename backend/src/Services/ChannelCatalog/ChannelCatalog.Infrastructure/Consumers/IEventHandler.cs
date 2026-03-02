namespace ChannelCatalog.Infrastructure.Consumers;

public interface IEventHandler
{
    string RoutingKey { get; }
    Task HandleAsync(string payloadJson, CancellationToken ct);
}
