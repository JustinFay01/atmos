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
    public static IServiceCollection UseAtmosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AtmosContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadingRepository, ReadingRepository>();
        services.AddSingleton<ISensorClient, MockSensorClient>();

        return services;
    }
}
