namespace Deals.UseCases.Deals.DecideDeal;

public sealed record PublisherDecideDealCommand(Guid PublisherUserId, Guid DealId, bool Accept);
