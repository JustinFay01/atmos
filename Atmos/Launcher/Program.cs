using Launcher.Commands;

using Spectre.Console.Cli;

namespace Launcher;

internal abstract class Program
{
    public static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<InstallCommand>();
        app.Configure(config =>
        {
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
        });
        
        return await app.RunAsync(args);
    }
}

