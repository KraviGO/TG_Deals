namespace Payments.UseCases.TopUps.CreateTopUp;

public sealed record CreateTopUpResult(Guid TopUpId, string ConfirmationUrl);
