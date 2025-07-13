using System.CommandLine;

using Launcher.Menu;
using Launcher.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Launcher;

internal abstract class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Atmos CLI Launcher");
        var debugOption = new Option<bool>("--debug");
        rootCommand.Options.Add(debugOption);
        rootCommand.SetAction(async result =>
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Logging.ClearProviders();
            var debug = result.GetValue(debugOption);
            builder.Services.AddSingleton<LauncherContext>(
                new LauncherContext
                {
                    DebugMode = debug
                });
            
            builder.Services.AddHostedService<MenuManager>(); 
            using var host = builder.Build();
            await host.RunAsync();
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
