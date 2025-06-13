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

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IRealtimeUpdateNotifier, RealTimeUpdateNotifier>();
        builder.Services.UseAtmosInfrastructure();
        builder.Services.UseAtmosApplicationServices();
        builder.Services.AddHostedService<SensorPollingWorker>();
        builder.Services.AddControllers();

        var app = builder.Build();
        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        app.MapHub<DashboardHub>("/dashboard");
        app.MapControllers();
        app.Run();
    }
}
