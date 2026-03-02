namespace Deals.UseCases.Deals.PayDeal;

public sealed record PayDealResult(Guid PaymentId, string ConfirmationUrl);
