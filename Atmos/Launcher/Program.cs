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

        var rootCommand = new RootCommand("Atmos CLI Launcher")
        {
            installCommand,
            debugOption
        };
        
        installCommand.Options.Add(debugOption);
        installCommand.SetAction(async result =>
        {
            var handler = builder.BuildDefaultChain();
            var debugMode = result.GetValue(debugOption);
            var executorOptions = new ExecutorOptions { DebugMode = debugMode };
            var handlerResult = await executor.Execute(handler, executorOptions);
            Environment.Exit(handlerResult.ExitCode);
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
