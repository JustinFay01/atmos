using Launcher.Handlers;

namespace Launcher.Services;

public class ChainBuilder
{
    public IInstallationHandler BuildInstallChain()
    {
        var rootHandler = new CheckForExistingInstallationHandler();

        rootHandler
            .SetNext(new PathPromptHandler())
            .SetNext(new FetchReleaseInfoHandler())
            .SetNext(new DownloadReleaseHandler())
            .SetNext(new UnzipHandler())
            .SetNext(new DockerComposeHandler())
            .SetNext(new RunMigrationsHandler());

        return rootHandler;
    }
    
    public IInstallationHandler BuildUpdateChain()
    {
        var rootHandler = new CheckForExistingInstallationHandler();

        rootHandler
            .SetNext(new FetchReleaseInfoHandler())
            .SetNext(new DownloadReleaseHandler())
            .SetNext(new UnzipHandler())
            .SetNext(new DockerComposeHandler())
            .SetNext(new RunMigrationsHandler());

        return rootHandler;
    }
}
