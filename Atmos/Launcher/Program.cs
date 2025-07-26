using System.CommandLine;

using Launcher.Extensions;
using Launcher.Handlers.Attributes;
using Launcher.Menu;
using Launcher.Models;
using Launcher.Services;

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
            builder.Services.RegisterHandlers();
            
            var debug = result.GetValue(debugOption);
            builder.Services.AddSingleton<IAtmosLogService, AtmosLogService>();
            builder.Services.AddSingleton<ChainBuilder>();
            builder.Services.AddSingleton(
                new LauncherContext
                {
                    DebugMode = debug
                });
            
            using var host = builder.Build();
            
            var executor = new ChainExecutor();
            var chainBuilder = host.Services.GetRequiredService<ChainBuilder>();
            var chain = chainBuilder.BuildChain(ChainType.Install);
            await executor.ExecuteInstallation(chain);
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
