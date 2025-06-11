using API.Hubs;

using Application;
using Application.Extensions;

using Infrastructure.Extensions;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();
        builder.Services.UseAtmosInfrastructure(builder.Configuration);
        builder.Services.UseApplicationServices();
        builder.Services.AddHostedService<SensorPollingWorker>();

        var app = builder.Build();

        app.MapHub<DashboardHub>("/dashboard");

        app.Run();
    }
}
