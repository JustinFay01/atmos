using System.Runtime.InteropServices;

using Launcher.Handlers.Abstract;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Handlers;

/// <summary>
/// Checks if an Atmos config file already exists in the hidden folder path:
/// Windows: C:\Users\{username}\AppData\Roaming\Atmos
/// MacOS/Linux: ~/.atmos
///
/// If so, it saves the previous user-selected installation path to the context and
/// sets the PreviouslyInstalled flag to true.
/// </summary>
public class CheckForExistingInstallationHandler : DefaultSetNextHandler
{
    public override string StepName => "Checking for existing installation";

    public override async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        var configService = new AtmosConfigService();
        var existingConfig = await configService.LoadConfigAsync();
        context.Config = existingConfig;
        
        // For now, an empty string means it is a silent step.
        return new HandlerResult(true, "");
    }
}
