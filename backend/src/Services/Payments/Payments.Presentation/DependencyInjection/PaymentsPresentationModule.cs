using Microsoft.Extensions.DependencyInjection;

using Payments.UseCases.PublisherWallet.GetMyPublisherLedgerEntries;
using Payments.UseCases.PublisherWallet.GetMyPublisherWallet;
using Payments.UseCases.PublisherWallet.WithdrawPublisherBalance;
using Payments.UseCases.TopUps.GetMyTopUps;
using Payments.UseCases.TopUps.CreateTopUp;
using Payments.UseCases.TopUps.ProcessTopUpWebhook;
using Payments.UseCases.Webhooks.ProcessYooKassaWebhook;
using Payments.UseCases.Wallet.CaptureReservation;
using Payments.UseCases.Wallet.GetMyWallet;
using Payments.UseCases.Wallet.GetMyWalletTransactions;
using Payments.UseCases.Wallet.InternalCreditWallet;
using Payments.UseCases.Wallet.ReleaseReservation;
using Payments.UseCases.Wallet.ReserveForDeal;
using Payments.UseCases.Wallet.WithdrawFromWallet;

namespace Payments.Presentation.DependencyInjection;

public static class PaymentsPresentationModule
{
    public static IServiceCollection AddPaymentsPresentation(this IServiceCollection services)
    {

        services.AddScoped<GetMyWalletHandler>();
        services.AddScoped<GetMyWalletTransactionsHandler>();
        services.AddScoped<CreateTopUpHandler>();
        services.AddScoped<GetMyTopUpsHandler>();
        services.AddScoped<ProcessTopUpWebhookHandler>();
        services.AddScoped<ProcessYooKassaWebhookHandler>();
        services.AddScoped<ReserveForDealHandler>();
        services.AddScoped<ReleaseReservationHandler>();
        services.AddScoped<CaptureReservationHandler>();
        services.AddScoped<InternalCreditWalletHandler>();
        services.AddScoped<WithdrawFromWalletHandler>();
        services.AddScoped<GetMyPublisherWalletHandler>();
        services.AddScoped<GetMyPublisherLedgerEntriesHandler>();
        services.AddScoped<WithdrawPublisherBalanceHandler>();
        return services;
    }
}
