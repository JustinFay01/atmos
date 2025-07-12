using Launcher.Handlers.Abstract;

using Octokit;

using Spectre.Console;
using Spectre.Console.Extensions;

namespace Launcher.Handlers;

public class FetchReleaseInfoHandler : DefaultSetNextHandler, IInstallationHandler
{
    private const string GitHubOwner = "JustinFay01";
    private const string GitHubRepo = "atmos";

    public override string StepName => "Fetching latest release information";

    public override async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        AnsiConsole.MarkupLine($"[yellow]{StepName}[/]");
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

            context.FetchedVersionTag = latestRelease.TagName;
            context.ReleaseAssetUrl = releaseAsset.BrowserDownloadUrl;
            
            AnsiConsole.MarkupLine($"[green]Successfully fetched release information:[/]");
            AnsiConsole.MarkupLine($"[yellow]Version:[/] {context.FetchedVersionTag}");
            AnsiConsole.MarkupLine($"[yellow]Download URL:[/] {context.ReleaseAssetUrl}");

        }
        catch (NotFoundException ex)
        {
            AnsiConsole.WriteException(ex);
            return new HandlerResult(false, $"Could not find repository '{GitHubOwner}/{GitHubRepo}'. Is it public?");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return new HandlerResult(false, $"An unexpected error occurred while fetching from GitHub: {ex.Message}");
        }

        return new HandlerResult(true, "Successfully fetched release information.");
    }
}
