using Launcher.Handlers;
using Launcher.Models;

using Spectre.Console;

namespace Launcher.Services;


public class ChainExecutor
{
    public async Task<HandlerResult> ExecuteSilentChainAsync(IHandler? chain, CancellationToken cancellationToken = default)
    {
        if (chain == null)
        {
            return HandlerResult.Failure("Installation chain is empty.");
        }

        var currentHandler = chain;

        while (currentHandler != null && !cancellationToken.IsCancellationRequested)
        {
            var result = await currentHandler.HandleAsync();
            if (!result.IsSuccess)
            {
                return result; 
            }
            
            currentHandler = currentHandler.Next;
        }

        return HandlerResult.Success("Installation completed successfully.");
    }
     public async Task<HandlerResult> ExecuteInstallation(IHandler? chain)
    {
        if (chain == null)
        {
            return HandlerResult.Failure("Installation chain is empty.");
        }

        AnsiConsole.Write(new Rule("[bold yellow]Atmos Installer[/]").Centered());
        var currentHandler = chain;
        var stepNumber = 1;

        while (currentHandler != null)
        {
            HandlerResult result;

            // Check if the handler is interactive
            if (currentHandler is IInteractiveHandler)
            {
                result = await currentHandler.HandleAsync();
            }
            else
            {
                // --- BACKGROUND HANDLER LOGIC (with spinner) ---
                result = HandlerResult.Failure("Handler did not run."); // Default result

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync($"[cyan]Step {stepNumber}: {currentHandler.StepName}...[/]", async ctx =>
                    {
                        // Quietly execute the handler's logic
                        result = await currentHandler.HandleAsync();
                        if (!result.IsSuccess)
                        {
                            ctx.Status($"[red]Failed: {currentHandler.StepName}[/]");
                            ctx.Spinner(Spinner.Known.Arc);
                        }
                    });
            }

            // --- COMMON RESULT HANDLING ---
            if (result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[green]✓[/] [dim]Step {stepNumber}: {currentHandler.StepName} finished.[/]");
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    AnsiConsole.MarkupLine($"  [grey]=> {result.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]✗ Step {stepNumber}: {currentHandler.StepName} failed.[/]");
                var errorPanel = new Panel($"[white]{result.Message}[/]")
                    .Header("[bold red]ERROR[/]").BorderColor(Color.Red).Expand();
                AnsiConsole.Write(errorPanel);
                return result; // Stop on failure
            }
            
            currentHandler = currentHandler.Next;
            stepNumber++;
        }

        AnsiConsole.Write(new Rule("[bold green]Installation Complete[/]").Centered());
        return HandlerResult.Success("Installation completed successfully.");
    }
}
