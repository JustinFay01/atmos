using System.CommandLine;
using Launcher.Services;

namespace Launcher;

internal abstract class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = new ChainBuilder();
        var executor = new ChainExecutor();

        var debugOption = new Option<bool>("--debug");
        var installCommand = new Command("install", "Install or update the Atmos Client.");
        var updateCommand = new Command("update", "Update the Atmos Client to the latest version.")
        {
            debugOption
        };

        var rootCommand = new RootCommand("Atmos CLI Launcher")
        {
            installCommand,
            updateCommand,
        };
        
        installCommand.Options.Add(debugOption);
        installCommand.SetAction(async result =>
        {
            var handler = builder.BuildInstallChain();
            var debugMode = result.GetValue(debugOption);
            var executorOptions = new ExecutorOptions { DebugMode = debugMode };
            var handlerResult = await executor.Execute(handler, executorOptions);
            Environment.Exit(handlerResult.ExitCode);
        });
        
        updateCommand.SetAction(async result =>
        {
            var handler = builder.BuildUpdateChain();
            var debugMode = result.GetValue(debugOption);
            var executorOptions = new ExecutorOptions { DebugMode = debugMode };
            var handlerResult = await executor.Execute(handler, executorOptions);
            Environment.Exit(handlerResult.ExitCode);
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
