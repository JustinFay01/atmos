using Application.Interfaces;

using Infrastructure.Hardware;

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

        services.AddSingleton<ISensorClient, MockSensorClient>();

        return services;
    }
}
