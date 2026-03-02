namespace Payments.Entities.Payments;

public enum PaymentStatus
{
    Created = 1,
    PendingUserAction = 2,
    Authorized = 3,
    Captured = 4,
    Canceled = 5
}
