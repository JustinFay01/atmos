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
                throw new ArgumentException("Connection string cannot be null or empty.");
            }
            options.UseNpgsql(connectionString);
        });
        

        services.AddScoped<IReadingAggregateRepository, ReadingAggregateRepository>();
        services.AddSingleton(CreateSensorClient(configuration));

        return services;
    }
    
    private static ISensorClient CreateSensorClient(IConfiguration configuration)
    {
        var sensorSettings = configuration.GetSection("SensorSettings");
        
        var sensorType = sensorSettings["Type"] ?? "Real";
        var portName = sensorSettings["PortName"] ?? "COM3";
        var baudRate = int.TryParse(sensorSettings["BaudRate"], out var br) ? br : 57600;
        var parity = sensorSettings["Parity"] ?? "None";
        var dataBits = int.TryParse(sensorSettings["DataBits"], out var db) ? db : 8;
        var stopBits = sensorSettings["StopBits"] ?? "One";
        var readTimeout = int.TryParse(sensorSettings["ReadTimeout"], out var rt) ? rt : 5000;
        var writeTimeout = int.TryParse(sensorSettings["WriteTimeout"], out var wt) ? wt : 5000;
        
        return sensorType switch
        {
            "Real" => new Rs485SensorClient(
                portName, baudRate, parity, dataBits, stopBits, readTimeout, writeTimeout
                ),
            "Mock" => new MockSensorClient(),
            _ => throw new ArgumentException($"Unsupported sensor type: {sensorType}. Supported types: Real, Mock.", "SensorSettings:Type")
        };
    }
}
