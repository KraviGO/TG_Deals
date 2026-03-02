using Deals.UseCases.Deals.CancelDeal;
using Deals.UseCases.Deals.CreateDeal;
using Deals.UseCases.Deals.DecideDeal;
using Deals.UseCases.Deals.GetMyDeals;
using Deals.UseCases.Deals.GetPublisherDeals;
using Deals.UseCases.Deals.PayDeal;
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
        services.AddScoped<CancelDealHandler>();
        services.AddScoped<PayDealHandler>();
        return services;
    }
}
