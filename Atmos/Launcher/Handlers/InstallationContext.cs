namespace Launcher.Handlers;

public class InstallationContext
{
    /// <summary>
    /// The absolute path where the application will be installed.
    /// This is set by the PathPromptHandler and used by subsequent handlers.
    /// </summary>
    public string InstallationPath { get; set; } = string.Empty;
    
    /// <summary>
    /// The version tag fetched from the GitHub release (e.g., "v1.0.0").
    /// Set by FetchReleaseInfoHandler.
    /// </summary>
    public string FetchedVersionTag { get; set; } = string.Empty;
    
    /// <summary>
    /// The direct download URL for the .zip release asset.
    /// Set by FetchReleaseInfoHandler.
    /// </summary>
    public string ReleaseAssetUrl { get; set; } = string.Empty;
}
