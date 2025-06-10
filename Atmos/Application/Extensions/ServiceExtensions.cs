using Application.Interfaces;
using Application.Services;

namespace Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper profiles
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddSingleton<IAggregator, AggregatorService>();
        
        return services;
    }
}