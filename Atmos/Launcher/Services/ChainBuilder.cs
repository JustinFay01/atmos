using Launcher.Handlers;

namespace Launcher.Services;

public class ChainBuilder
{
    public IInstallationHandler BuildDefaultChain()
    {
        var rootHandler = new CheckForExistingInstallation();

        rootHandler
            .SetNext(new PathPromptHandler())
            .SetNext(new FetchReleaseInfoHandler());

        return rootHandler;
    }
}
