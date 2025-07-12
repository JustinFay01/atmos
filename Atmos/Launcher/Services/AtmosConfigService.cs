using System.Runtime.InteropServices;
using System.Text.Json;

using Launcher.Models;

using Spectre.Console;

namespace Launcher.Services;

public class AtmosConfigService
{
    private readonly string _configFilePath;

    public AtmosConfigService()
    {
        _configFilePath = GetConfigFilePath();
    }
    
    public async Task SaveConfigAsync(AtmosConfig config)
    {
        var jsonContent = JsonSerializer.Serialize(config);
        await File.WriteAllTextAsync(_configFilePath, jsonContent);
    }

    /// <summary>
    /// Asynchronously loads the AtomsConfig from the standard location.
    /// </summary>
    /// <returns>The loaded AtomsConfig object, or null if it doesn't exist or is invalid.</returns>
    public async Task<AtmosConfig?> LoadConfigAsync()
    {
        if (!File.Exists(_configFilePath))
        {
            return null; 
        }

        try
        {
            var jsonContent = await File.ReadAllTextAsync(_configFilePath);

            // Handle case where the file might be empty
            return string.IsNullOrWhiteSpace(jsonContent) ? null :
                JsonSerializer.Deserialize<AtmosConfig>(jsonContent);
        }
        catch (JsonException ex)
        {
            AnsiConsole.Write(
                $"Error: Configuration file at '{_configFilePath}' is malformed. Details: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            AnsiConsole.Write(
                $"Error: Could not read configuration file at '{_configFilePath}'. Details: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Determines the full path to the configuration file based on the OS.
    /// </summary>
    /// <returns>The full path to the config file.</returns>
    private string GetConfigFilePath()
    {
        var basePath = Environment.GetFolderPath(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            // Best practice for Windows: C:\Users\username\AppData\Roaming
            Environment.SpecialFolder.ApplicationData :
            // macOS, Linux, etc.
            // Best practice for Unix-like systems: ~/.atmos
            Environment.SpecialFolder.UserProfile);

        var appFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Atmos" // -> C:\Users\user\AppData\Roaming\Atmos
            : ".atmos"; // -> /home/user/.atmos

        // 3. Combine the base path with your application's folder name.
        var configFolderPath = Path.Combine(basePath, appFolderName);
        var configFilePath = Path.Combine(configFolderPath, "config.json"); // Or whatever your file is called
        
        // Ensure the directory exists
        Directory.CreateDirectory(configFolderPath);
        return configFilePath;
    }
}
