using System.Runtime.InteropServices;

using Launcher.Handlers.Abstract;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class PathPromptHandler : DefaultSetNextHandler, IInteractiveHandler
{
    public override string StepName => "Choose Installation Path";
    public PathPromptHandler(LauncherContext context) : base(context)
    {
    }
    public override async Task<HandlerResult> HandleAsync()
    {
        var fallbackPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Atmos")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".atmos");
        
        var configService = new AtmosConfigService();

        // Determine a sensible default path based on the operating system.
        if (!Directory.Exists(Context.Config.InstallPath))
        {
            var selectedPath = await SelectPathAsync(fallbackPath);
            Context.Config.InstallPath = selectedPath;
            await configService.SaveConfigAsync(Context.Config);
            return new HandlerResult(true, "Installation path selected.");
        }
        
        AnsiConsole.MarkupLine($"[blue]A previous installation was detected: {Context.Config.InstallPath}[/]");
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
        Context.Config.InstallPath = newPath;
        await configService.SaveConfigAsync(Context.Config);
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
        if (Directory.Exists(chosenPath))
        {
            return chosenPath;
        }

        try
        {
            Directory.CreateDirectory(chosenPath);
            if (Context.DebugMode)
            {
                AnsiConsole.MarkupLine($"[green]✓[/] Created directory: [blue]{chosenPath}[/]");
            }
        }
        catch (Exception ex)
        {
            if (Context.DebugMode)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
            throw;
        }
        return chosenPath;
    }
}
