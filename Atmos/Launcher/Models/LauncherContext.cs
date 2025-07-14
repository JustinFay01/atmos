using System.Diagnostics;

namespace Launcher.Models;

public class LauncherContext
{
    /// <summary>
    /// Whether the launcher is running in debug mode.
    /// </summary>
    public bool DebugMode { get; set; } = false;
    
    /// <summary>
    /// If previously installed, this will contain the AtmosConfig.
    /// </summary>
    public AtmosConfig Config { get; set; } = new();
    
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
    
    /// <summary>
    /// Temporary path where the Atmos release zip file will be downloaded.
    /// </summary>
    public string TemporaryZipPath { get; set; } = string.Empty;
    
    public bool NewUpdateAvailable { get; set; }

    public Dictionary<string, Process> RunningProcesses { get; set; } = new();
}
