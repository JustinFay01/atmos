using API.Extensions;
using API.Hubs;

using Application;
using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Application.Workers;

using Infrastructure.Extensions;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.UseAtmosIssueTracker();

        // Allow cors
        var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(myAllowSpecificOrigins,
                corsPolicyBuilder =>
                {
                    corsPolicyBuilder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IRealtimeUpdateNotifier, RealTimeUpdateNotifier>();
        builder.Services.AddSingleton<SensorSettings>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var sensorSettings = new SensorSettings();
            configuration.GetSection("Sensor").Bind(sensorSettings);
            return sensorSettings;
        });
        
        builder.Services.UseAtmosInfrastructure(builder.Configuration);
        builder.Services.UseAtmosApplicationServices();
        builder.Services.AddHostedService<SensorPollingWorker>();
        builder.Services.AddControllers();

        var app = builder.Build();
        app.UseCors(myAllowSpecificOrigins);

        app.UseStaticFiles();


        app.MapHub<DashboardHub>("/v1/dashboard");
        app.MapControllers();
        app.MapFallbackToFile("index.html");
    
        app.Run();
    }
}
