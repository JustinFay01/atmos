using System.Runtime.InteropServices;

using Launcher.Handlers.Abstract;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class PathPromptHandler : DefaultSetNextHandler, IInteractiveInstallationHandler
{
    public override string StepName => "Choose Installation Path";
    public override async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        var fallbackPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Atmos")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".atmos");
        
        var configService = new AtmosConfigService();

        // Determine a sensible default path based on the operating system.
        if (context.Config == null || !Directory.Exists(context.Config.InstallPath))
        {
            context.Config ??= new AtmosConfig { InstallPath = fallbackPath };
            var selectedPath = await SelectPathAsync(fallbackPath);
            context.Config.InstallPath = selectedPath;
            await configService.SaveConfigAsync(context.Config);
            return new HandlerResult(true, "Installation path selected.");
        }
        
        AnsiConsole.MarkupLine($"[blue]A previous installation was detected: {context.Config.InstallPath}[/]");
        var usePreviousPath = await AnsiConsole.PromptAsync(
            new SelectionPrompt<string>()
                .Title("[yellow]Would you like to use this path?[/]")
                .AddChoices("Yes", "No")
        );
        
        if (usePreviousPath == "Yes")
        {
            return new HandlerResult(true, "Installation path selected.");
        }
        
        var newPath = await SelectPathAsync(fallbackPath);
        context.Config.InstallPath = newPath;
        await configService.SaveConfigAsync(context.Config);
        return new HandlerResult(true, "Installation path selected.");
    }
    
    private async Task<string> SelectPathAsync(string defaultPath)
    {
        // Use Spectre.Console's TextPrompt for a rich user experience.
        var chosenPath = await AnsiConsole.PromptAsync(
            new TextPrompt<string>("[green]Please enter the installation path:[/]")
                .DefaultValue(defaultPath) 
                .Validate(path => path.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                    ? ValidationResult.Error("[red]The path contains invalid characters.[/]")
                    :
                    // More validation could be added here (e.g., check for write permissions).
                    ValidationResult.Success()));
        
        // Ensure the directory exists, creating it if necessary.
        if (!Directory.Exists(chosenPath))
        {
            try
            {
                Directory.CreateDirectory(chosenPath);
                AnsiConsole.MarkupLine($"[green]✓[/] Created directory: [blue]{chosenPath}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error creating directory: {ex.Message}[/]");
                throw;
            }
        }
        return chosenPath;
    }
}
