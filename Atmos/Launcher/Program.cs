using System.Diagnostics.CodeAnalysis;

using Launcher.Commands;

using Spectre.Console.Cli;

namespace Launcher;

internal abstract class Program
{
    [RequiresDynamicCode("Calls Spectre.Console.Cli.CommandApp.CommandApp(ITypeRegistrar)")]
    public static async Task<int> Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<InstallCommand>("install")
                .WithDescription("Installs the Atmos CLI and its dependencies.")
                .WithExample("install");
            
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
        });
        
        return await app.RunAsync(args);
    }
}

