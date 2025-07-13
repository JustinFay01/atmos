using System.Runtime.InteropServices;

using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

/// <summary>
/// Checks if an Atmos config file already exists in the hidden folder path:
/// Windows: C:\Users\{username}\AppData\Roaming\Atmos
/// MacOS/Linux: ~/.atmos
///
/// If so, it saves the previous user-selected installation path to the context and
/// sets the PreviouslyInstalled flag to true.
/// </summary>
[HandlerOrder(ChainType.Install, 10)]
[HandlerOrder(ChainType.Update, 10)]
[HandlerOrder(ChainType.Initialization, 10)]
public class CheckForExistingInstallationHandler : DefaultSetNextHandler
{
    public override string StepName => "Checking for existing installation";

    public CheckForExistingInstallationHandler(LauncherContext context) : base(context)
    {
    }

    public override async Task<HandlerResult> HandleAsync()
    {
        var configService = new AtmosConfigService();
        var existingConfig = await configService.LoadConfigAsync();
        Context.Config = existingConfig;

        if (Context.DebugMode)
        {
            var message = existingConfig.IsEmpty ? "No existing config found." : $"Existing config found. {existingConfig}";
            AnsiConsole.MarkupInterpolated($"[yellow]Debug Mode: {message}[/]");
        }
        
        // For now, an empty string means it is a silent step.
        return new HandlerResult(true, "");
    }
}
