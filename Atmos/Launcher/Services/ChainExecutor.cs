using Launcher.Handlers;

using Spectre.Console;

namespace Launcher.Services;

public class ChainExecutor
{
     public async Task<HandlerResult> Execute(IInstallationHandler? chain)
    {
        if (chain == null)
        {
            return HandlerResult.Failure("Installation chain is empty.");
        }

        // A nice, centered title for the process
        AnsiConsole.Write(new Rule("[bold yellow]Atmos Installer[/]").Centered());

        var context = new InstallationContext();
        var currentHandler = chain;
        var stepNumber = 1;

        // I've slightly modified your loop to correctly handle the entire chain
        while (currentHandler != null)
        {
            HandlerResult result = HandlerResult.Failure("Handler did not run.");

            // Use Spectre.Console's Status API to show a spinner while the task runs
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync($"[cyan]Step {stepNumber}: {currentHandler.StepName}...[/]", async ctx =>
                {
                    // Execute the handler's logic. All console output is managed here.
                    result = await currentHandler.HandleAsync(context);

                    // Optional: You can update the spinner text upon completion
                    if (!result.IsSuccess)
                    {
                        // Visually indicate failure before we break
                        ctx.Status($"[red]Failed: {currentHandler.StepName}[/]");
                        ctx.Spinner(Spinner.Known.Arc);
                    }
                });

            // Now, handle the result outside the Status context
            if (result.IsSuccess)
            {
                // Use Markup for colored text. [green]✓[/] is a green checkmark.
                AnsiConsole.MarkupLine($"[green]✓[/] [dim]Step {stepNumber}: {currentHandler.StepName} finished.[/]");
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    AnsiConsole.MarkupLine($"  [grey]=> {result.Message}[/]");
                }
            }
            else
            {
                // Use Markup for the error. [red]✗[/] is a red cross.
                AnsiConsole.MarkupLine($"[red]✗ Step {stepNumber}: {currentHandler.StepName} failed.[/]");

                // Use a Panel for a visually distinct error message
                var errorPanel = new Panel($"[white]{result.Message}[/]")
                    .Header("[bold red]ERROR[/]")
                    .BorderColor(Color.Red)
                    .Expand();
                AnsiConsole.Write(errorPanel);

                // Stop the entire process on the first failure
                return result;
            }
            
            // Move to the next handler in the chain
            currentHandler = currentHandler.Next;
            stepNumber++;
        }

        AnsiConsole.Write(new Rule("[bold green]Installation Complete[/]").Centered());
        AnsiConsole.MarkupLine("\n[green]Atmos was successfully installed![/]\n");
        return HandlerResult.Success("Installation completed successfully.");
    }
}
