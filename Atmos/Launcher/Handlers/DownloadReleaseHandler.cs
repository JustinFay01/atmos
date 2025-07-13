using Launcher.Handlers.Abstract;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class DownloadReleaseHandler : DefaultSetNextHandler
{
    public DownloadReleaseHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Downloading Atmos release zip file";
    public override async Task<HandlerResult> HandleAsync()
    {
        
        if (string.IsNullOrEmpty(Context.ReleaseAssetUrl))
        {
            return HandlerResult.Failure("No release asset URL found. Please fetch release information first.");
        }
        
        try
        {
            var zipPath = await DownloadZipAsync();
            Context.TemporaryZipPath = zipPath;
            if (Context.DebugMode)
            {
                AnsiConsole.MarkupLine("[green]Successfully downloaded Atmos release zip file[/]");
            }
            
            return HandlerResult.Success("Atmos release zip file downloaded successfully.");
        }
        catch (Exception ex)
        {
            return HandlerResult.Failure($"Failed to download Atmos release zip file: {ex.Message}");
        }
    }

    /// <summary>
    /// Downloads the Atmos release zip file from the provided URL.
    /// Stores in a temporary location.
    /// </summary>
    /// <returns>The temporary location path</returns>
    private async Task<string> DownloadZipAsync()
    {
        using var client = new HttpClient();
        await using var fs = new FileStream(Path.Combine(Path.GetTempPath(), "atmos-release.zip"), FileMode.Create, FileAccess.Write, FileShare.None);
        var response = await client.GetAsync(Context.ReleaseAssetUrl);
        await response.Content.CopyToAsync(fs);
        await fs.FlushAsync();
        return fs.Name;
    }
}
