using Application.Interfaces;
using Application.Profiles;
using Application.Rules;
using Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseAtmosApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ReadingDtoProfile));

        services.AddScoped<IMetricUpdateRuleFactory, MetricUpdateRuleFactory>();
        services.AddSingleton<IAggregator, AggregatorService>();

        return services;
    }
}
