using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Install, 40)]
[HandlerOrder(ChainType.Update, 30)]
public class DownloadReleaseHandler : DefaultSetNextHandler
{
    public DownloadReleaseHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Downloading Atmos release zip file";
    public override async Task<HandlerResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        
        if (string.IsNullOrEmpty(Context.ReleaseAssetUrl))
        {
            return HandlerResult.Failure("No release asset URL found. Please fetch release information first.");
        }
        
        try
        {
            var zipPath = await DownloadZipAsync(cancellationToken);
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
    private async Task<string> DownloadZipAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        await using var fs = new FileStream(Path.Combine(Path.GetTempPath(), "atmos-release.zip"), FileMode.Create, FileAccess.Write, FileShare.None);
        var response = await client.GetAsync(Context.ReleaseAssetUrl, cancellationToken);
        await response.Content.CopyToAsync(fs, cancellationToken);
        await fs.FlushAsync(cancellationToken);
        return fs.Name;
    }
}
