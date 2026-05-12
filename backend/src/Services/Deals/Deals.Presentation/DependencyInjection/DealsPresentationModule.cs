using Deals.UseCases.Deals.CancelDeal;
using Deals.UseCases.Deals.AdvertiserConfirm;
using Deals.UseCases.Deals.ConfirmPublished;
using Deals.UseCases.Deals.CreateDeal;
using Deals.UseCases.Deals.DecideDeal;
using Deals.UseCases.Deals.Disputes.GetAdminDisputes;
using Deals.UseCases.Deals.Disputes.OpenDispute;
using Deals.UseCases.Deals.Disputes.ResolveDispute;
using Deals.UseCases.Deals.GetMyDeals;
using Deals.UseCases.Deals.GetPublisherDeals;
using Microsoft.Extensions.DependencyInjection;

namespace Deals.Presentation.DependencyInjection;

public static class DealsPresentationModule
{
    public static IServiceCollection AddDealsPresentation(this IServiceCollection services)
    {
        services.AddScoped<CreateDealHandler>();
        services.AddScoped<GetMyDealsHandler>();
        services.AddScoped<GetPublisherDealsHandler>();
        services.AddScoped<PublisherDecideDealHandler>();
        services.AddScoped<ConfirmPublishedHandler>();
        services.AddScoped<AdvertiserConfirmHandler>();
        services.AddScoped<CancelDealHandler>();
        services.AddScoped<GetAdminDisputesHandler>();
        services.AddScoped<OpenDealDisputeHandler>();
        services.AddScoped<ResolveDealDisputeHandler>();
        return services;
    }
}
