using System.Text.Json.Serialization;

namespace Launcher.Models;

public class AtmosConfig
{
    /// <summary>
    ///  The path where the Atmos client is installed.
    /// </summary>
    [JsonPropertyName("install_path")]
    public string InstallPath { get; set; } = string.Empty;
    
    /// <summary>
    /// The version of the Atmos CLI tool.
    /// </summary>
    [JsonPropertyName("cli_version")]
    public string CliVersion { get; set; } = string.Empty;
    
    /// <summary>
    /// The version of the Atmos client.
    /// </summary>
    [JsonPropertyName("atmos_version")]
    public string AtmosVersion { get; set; } = string.Empty;
    
    public bool IsEmpty => string.IsNullOrEmpty(InstallPath) && string.IsNullOrEmpty(CliVersion) && string.IsNullOrEmpty(AtmosVersion);

    public override string ToString()
    {
        return $"AtmosConfig(InstallPath: {InstallPath}, CliVersion: {CliVersion}, AtmosVersion: {AtmosVersion})";
    }
}
