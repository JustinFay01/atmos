using Application.Interfaces;

using Domain.Interfaces;

using Infrastructure.Hardware;
using Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseAtmosInfrastructure(this IServiceCollection services)
    {

        services.AddDbContext<AtmosContext>();

        services.AddScoped<IReadingAggregateRepository, ReadingAggregateRepository>();
        services.AddSingleton<ISensorClient, MockSensorClient>();

        return services;
    }
}
