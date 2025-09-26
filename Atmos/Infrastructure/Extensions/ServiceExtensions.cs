using Application.Interfaces;

using Domain.Interfaces;

using Infrastructure.Hardware;
using Infrastructure.Logs;
using Infrastructure.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection UseAtmosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IReadingAggregateRepository, ReadingAggregateRepository>();
        services.AddScoped<IReadingLogWriter, CsvReadingLogWriter>();
        services.AddSingleton(CreateSensorClient(configuration));

        return services;
    }
    
    private static ISensorClient CreateSensorClient(IConfiguration configuration)
    {
        var sensorSettings = configuration.GetSection("Sensor");
        
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
