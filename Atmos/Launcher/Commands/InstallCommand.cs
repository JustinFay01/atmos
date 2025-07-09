using System.ComponentModel;

using Launcher.Services;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Launcher.Commands;

public sealed class InstallSettings : CommandSettings
{
    [CommandOption("-i|--install")]
    [DefaultValue(true)]
   public bool Install { get; set; }
   
}

public class InstallCommand : AsyncCommand<InstallSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, InstallSettings settings)
    {
        if (!settings.Install)
        {
            AnsiConsole.MarkupLine("[yellow]Installation skipped.[/]");
            return 0;
        }

        var builder = new ChainBuilder();
        var executor = new ChainExecutor();

        var chain = builder.BuildDefaultChain();

        var result = await executor.Execute(chain);

        if (result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"\n[bold green]Installation successful![/]");
            return 0;
        }
        
        AnsiConsole.MarkupLine($"\n[bold red]Error:[/] {result.Message}");
        return -1; // Failure
    }
    
    
}
