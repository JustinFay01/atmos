using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Initialization, 30)]
public class NewUpdateAvailableHandler : DefaultSetNextHandler
{
    public NewUpdateAvailableHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Checking for new update";
    public override Task<HandlerResult> HandleAsync()
    {
        // Check if the fetched version is different from the current version
        if (Context.FetchedVersionTag != Context.Config.AtmosVersion)
        {
            Context.NewUpdateAvailable = true;
        }
        return Task.FromResult(HandlerResult.Success(""));

    }
}
