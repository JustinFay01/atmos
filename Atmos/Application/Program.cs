namespace Application;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<SensorPollingWorker>();

        var host = builder.Build();
        host.Run();
    }
}
