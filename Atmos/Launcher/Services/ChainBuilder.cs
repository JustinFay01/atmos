using Launcher.Handlers;

namespace Launcher.Services;

public class ChainBuilder
{
    public IInstallationHandler BuildDefaultChain()
    {
        var rootHandler = new CheckForExistingInstallationHandler();

        rootHandler
            .SetNext(new PathPromptHandler())
            .SetNext(new FetchReleaseInfoHandler())
            .SetNext(new DownloadReleaseHandler())
            .SetNext(new UnzipHandler());

        return rootHandler;
    }
}
