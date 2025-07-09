using Launcher.Handlers;

namespace Launcher.Services;

public class ChainExecutor
{
    public async Task<HandlerResult> Execute(IInstallationHandler chain)
    {
        var result = await chain.HandleAsync(new InstallationContext());

        return result;
    }

}
