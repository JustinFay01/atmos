

using System.CommandLine;

using Launcher.Services;

namespace Launcher;

internal abstract class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = new ChainBuilder();
        var executor = new ChainExecutor();

        var installOption = new Option<bool>("--install")
        {
            Description = "Install the Atmos Client. If not specified, the tool will not be installed.",
        };
        
        var rootCommand = new RootCommand("Atmos CLI Launcher")
        {
            Description = "A launcher for the Atmos CLI tool, handling installation, updates, and setup."
        };
        rootCommand.Options.Add(installOption);
        rootCommand.SetAction(async result =>
        {
            var install = result.GetValue(installOption);
            if (install)
            {
                var handlerResult = await executor.Execute(builder.BuildDefaultChain());
                Environment.Exit(handlerResult.ExitCode);   
            }
        });
        
        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}

