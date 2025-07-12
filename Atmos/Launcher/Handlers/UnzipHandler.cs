using Launcher.Handlers.Abstract;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class UnzipHandler : DefaultSetNextHandler, IInteractiveInstallationHandler
{
    public override string StepName => "Unzipping Atmos release zip file";
    public override async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        AnsiConsole.MarkupLine($"[yellow]{StepName}[/]");
        if (string.IsNullOrEmpty(context.TemporaryZipPath) || !File.Exists(context.TemporaryZipPath))
        {
            return HandlerResult.Failure("No temporary zip path found. Please download the Atmos release zip file first.");
        }
        try
        {
            var extractPath = context.Config!.InstallPath;
            if (Directory.Exists(extractPath) && Directory.GetFiles(extractPath).Length > 0)
            {
                var emptyDir = await AnsiConsole.PromptAsync(
                    new SelectionPrompt<string>()
                        .Title("[yellow]There are files in the directory, would you like to overwrite them?[/]")
                        .AddChoices("Yes", "No"));
                        
                if (emptyDir == "No")
                {
                    return HandlerResult.Failure("Please choose an empty directory for installation.");
                }
                
                // If the user chooses to overwrite, delete the existing files
                Directory.Delete(extractPath, true);
            }
            
            Directory.CreateDirectory(extractPath);
            
            System.IO.Compression.ZipFile.ExtractToDirectory(context.TemporaryZipPath, extractPath);
            AnsiConsole.MarkupLine($"[green]Successfully unzipped Atmos release zip file to {extractPath}[/]");
            
            var configService = new AtmosConfigService();
            context.Config!.AtmosVersion = context.FetchedVersionTag;
            await configService.SaveConfigAsync(context.Config);
            
            return HandlerResult.Success("Atmos release zip file unzipped successfully.");
        }
        catch (Exception ex)
        {
            return HandlerResult.Failure($"Failed to unzip Atmos release zip file: {ex.Message}");
        }
    }
}
