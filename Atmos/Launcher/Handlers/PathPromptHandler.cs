using System.Runtime.InteropServices;

using Spectre.Console;

namespace Launcher.Handlers;

public class PathPromptHandler : IInstallationHandler
{
    public string StepName => "PChoose Installation Path";
    private IInstallationHandler? _nextHandler;
    
    public IInstallationHandler SetNext(IInstallationHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        AnsiConsole.MarkupLine($"\n[bold underline yellow]{StepName}[/]");

        // Determine a sensible default path based on the operating system.
        var defaultPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"C:\Atmos"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "atmos");

        // Use Spectre.Console's TextPrompt for a rich user experience.
        var chosenPath = await AnsiConsole.PromptAsync(
            new TextPrompt<string>("[green]Please enter the installation path:[/]")
                .DefaultValue(defaultPath) // Provide a smart default
                .Validate(path => path.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ? ValidationResult.Error("[red]The path contains invalid characters.[/]") :
                    // More validation could be added here (e.g., check for write permissions).
                    ValidationResult.Success())
        );
        
        // Store the chosen path in our shared context object.
        context.InstallationPath = Path.GetFullPath(chosenPath); // Normalize to an absolute path
        
        // Check if a config file already exists at the chosen path.
        if (File.Exists(Path.Combine(context.InstallationPath, "config.json")))
        {
            AnsiConsole.MarkupLine("[blue]A configuration file already exists at this path. Do you want to overwrite it?[/]");
            var overwrite = await AnsiConsole.PromptAsync(
                new SelectionPrompt<string>()
                    .Title("[yellow]Do you want to overwrite the existing configuration?[/]")
                    .AddChoices("Yes", "No")
            );
            if (overwrite == "No")
            {
                AnsiConsole.MarkupLine("[red]Installation cancelled by user.[/]");
                return new HandlerResult(false, "Installation cancelled by user.");
            }
        }
        
        // Save the installation path to a configuration file or similar storage.
        var configFilePath = Path.Combine(context.InstallationPath, "config.json");
        Directory.CreateDirectory(context.InstallationPath); // Ensure the directory exists
        await File.WriteAllTextAsync(configFilePath, $"{{ \"installationPath\": \"{context.InstallationPath}\" }}");
        

        // Provide immediate feedback to the user.
        AnsiConsole.MarkupLine($"[green]✓[/] Installation will proceed in: [blue]{context.InstallationPath}[/]\n");

        // The interactive step is done. Now pass control to the next (non-interactive) handler.
        if (_nextHandler != null)
        {
            return await _nextHandler.HandleAsync(context);
        }

        return new HandlerResult(true, "Installation path selected.");
    }
}
