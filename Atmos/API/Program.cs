using API.Extensions;
using API.Hubs;

using Application;
using Application.Extensions;
using Application.Interfaces;
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
        builder.Services.UseAtmosInfrastructure(builder.Configuration);
        builder.Services.UseAtmosApplicationServices();
        builder.Services.AddHostedService<SensorPollingWorker>();
        builder.Services.AddControllers();

        var app = builder.Build();
        app.UseCors(myAllowSpecificOrigins);

        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        app.MapHub<DashboardHub>("/v1/dashboard");
        app.MapControllers();
        app.Run();
    }
}
