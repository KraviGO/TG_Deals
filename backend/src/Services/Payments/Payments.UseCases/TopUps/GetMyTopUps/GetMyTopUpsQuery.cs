namespace Payments.UseCases.TopUps.GetMyTopUps;

public sealed record GetMyTopUpsQuery(Guid UserId, int Limit = 100);
