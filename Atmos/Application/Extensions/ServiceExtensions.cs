using Atmos.Application.Interfaces;
using Atmos.Application.Profiles;
using Atmos.Application.Rules;
using Atmos.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Atmos.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseAtmosApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(RawSensorReadingToAggregateProfile));
        services.AddAutoMapper(typeof(ReadingAggregateDtoProfile));

        services.AddSingleton<IMetricUpdateRuleFactory, MetricUpdateRuleFactory>();
        services.AddSingleton<IAggregator, AggregatorService>();
        services.AddSingleton<IHourlyReadingService, HourlyReadingService>();

        return services;
    }
}
