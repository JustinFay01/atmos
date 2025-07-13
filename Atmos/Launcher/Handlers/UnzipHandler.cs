using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Install, 50)]
[HandlerOrder(ChainType.Update, 40)]
public class UnzipHandler : DefaultSetNextHandler, IInteractiveHandler
{
    public UnzipHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Unzipping Atmos release zip file";
    public override async Task<HandlerResult> HandleAsync()
    {
        if (string.IsNullOrEmpty(Context.TemporaryZipPath) || !File.Exists(Context.TemporaryZipPath))
        {
            return HandlerResult.Failure("No temporary zip path found. Please download the Atmos release zip file first.");
        }
        try
        {
            var extractPath = Context.Config!.InstallPath;
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
            
            System.IO.Compression.ZipFile.ExtractToDirectory(Context.TemporaryZipPath, extractPath);
            if (Context.DebugMode)
            {
                AnsiConsole.MarkupLine($"[green]Successfully unzipped Atmos release zip file to {extractPath}[/]");
            }
            
            var configService = new AtmosConfigService();
            Context.Config.AtmosVersion = Context.FetchedVersionTag;
            await configService.SaveConfigAsync(Context.Config);
            
            return HandlerResult.Success("Atmos release zip file unzipped successfully.");
        }
        catch (Exception ex)
        {
            if (Context.DebugMode)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
            return HandlerResult.Failure($"Failed to unzip Atmos release zip file: {ex.Message}");
        }
    }
}
