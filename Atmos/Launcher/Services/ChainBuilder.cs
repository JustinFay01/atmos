using Launcher.Handlers;

namespace Launcher.Services;

public class ChainBuilder
{
    public IInstallationHandler BuildDefaultChain()
    {
        var pathPrompt = new PathPromptHandler();

        //dockerCheck.SetNext();

        return pathPrompt;
    }
}
