namespace Launcher.Handlers;

public class InstallationContext
{
    /// <summary>
    /// The absolute path where the application will be installed.
    /// This is set by the PathPromptHandler and used by subsequent handlers.
    /// </summary>
    public string InstallationPath { get; set; } = string.Empty;

}
