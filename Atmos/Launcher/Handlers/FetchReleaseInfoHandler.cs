using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Octokit;

using Spectre.Console;
using Spectre.Console.Extensions;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Install, 30)]
[HandlerOrder(ChainType.Update, 20)]
[HandlerOrder(ChainType.Initialization, 20)]
public class FetchReleaseInfoHandler : DefaultSetNextHandler, IHandler
{
    private const string GitHubOwner = "JustinFay01";
    private const string GitHubRepo = "atmos";

    public FetchReleaseInfoHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Fetching latest release information";

    public override async Task<HandlerResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Octokit requires a product name for the user-agent header.
            var client = new GitHubClient(new ProductHeaderValue("JustinFay01"));

            var releases = await client.Repository.Release.GetAll(GitHubOwner, GitHubRepo)
                .Spinner();
            
            if (releases == null || !releases.Any())
            {
                return new HandlerResult(false, "No releases found in the specified repository.");
            }
            
            var latestRelease = releases
                //.Where(r => !r.Prerelease) // Exclude pre-releases
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();
            
            if (latestRelease == null)
            {
                 return new HandlerResult(false, "No releases found in the specified repository.");
            }

            // Find the first asset that is a .zip file.
            var releaseAsset = latestRelease.Assets.FirstOrDefault(a => 
                a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));

            if (releaseAsset == null)
            {
                return new HandlerResult(false, $"Release '{latestRelease.TagName}' found, but it contains no .zip asset.");
            }

            Context.FetchedVersionTag = latestRelease.TagName;
            Context.ReleaseAssetUrl = releaseAsset.BrowserDownloadUrl;


            if (Context.DebugMode)
            {
                var panelMessage = new Markup($"[yellow]Version:[/] {Context.FetchedVersionTag}\n" +
                    $"[yellow]Download URL:[/] {Context.ReleaseAssetUrl}");
                var panel = new Panel(panelMessage)
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader("Release Information"),
                    Expand = true
                };
                AnsiConsole.Write(panel);
            }

        }
        catch (NotFoundException ex)
        {
            if (Context.DebugMode)
            {
                AnsiConsole.WriteException(ex);
            }
            return new HandlerResult(false, $"Could not find repository '{GitHubOwner}/{GitHubRepo}'. Is it public?");
        }
        catch (Exception ex)
        {
            if (Context.DebugMode)
            {
                AnsiConsole.WriteException(ex);
            }            
            return new HandlerResult(false, $"An unexpected error occurred while fetching from GitHub: {ex.Message}");
        }

        return new HandlerResult(true, "Successfully fetched release information.");
    }
}
