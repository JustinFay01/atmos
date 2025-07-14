using Application.Interfaces;

using Domain.Interfaces;

using Infrastructure.Hardware;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseAtmosInfrastructure(this IServiceCollection services, IConfiguration configuration, string? cliConnectionString = null)
    {
        var connectionString = 
            cliConnectionString ??
            configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AtmosContext>(options =>
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadingAggregateRepository, ReadingAggregateRepository>();
        services.AddSingleton<ISensorClient, MockSensorClient>();

        return services;
    }
}
