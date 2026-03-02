namespace Payments.UseCases.TopUps.CreateTopUp;

public sealed record CreateTopUpCommand(Guid UserId, decimal Amount, string Currency);
