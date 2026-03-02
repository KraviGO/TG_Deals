using Microsoft.Extensions.DependencyInjection;
using Payments.UseCases.Payments.CapturePayment;
using Payments.UseCases.Payments.CreatePayment;
using Payments.UseCases.Payments.ProcessWebhook;
using Payments.UseCases.TopUps.CreateTopUp;
using Payments.UseCases.TopUps.ProcessTopUpWebhook;
using Payments.UseCases.Wallet.CaptureReservation;
using Payments.UseCases.Wallet.GetMyWallet;
using Payments.UseCases.Wallet.ReleaseReservation;
using Payments.UseCases.Wallet.ReserveForDeal;

namespace Payments.Presentation.DependencyInjection;

public static class PaymentsPresentationModule
{
    public static IServiceCollection AddPaymentsPresentation(this IServiceCollection services)
    {
        services.AddScoped<CreatePaymentHandler>();
        services.AddScoped<CapturePaymentHandler>();
        services.AddScoped<ProcessWebhookHandler>();
        services.AddScoped<GetMyWalletHandler>();
        services.AddScoped<CreateTopUpHandler>();
        services.AddScoped<ProcessTopUpWebhookHandler>();
        services.AddScoped<ReserveForDealHandler>();
        services.AddScoped<ReleaseReservationHandler>();
        services.AddScoped<CaptureReservationHandler>();
        return services;
    }
}
