using Launcher.Handlers;
using Launcher.Models;

namespace Launcher.Services;

public class ChainBuilder
{
    private readonly LauncherContext _context;
    public ChainBuilder(LauncherContext context)
    {
        _context = context;
    }
    public IHandler BuildInstallChain()
    {
        var rootHandler = new CheckForExistingInstallationHandler(_context);

        rootHandler
            .SetNext(new PathPromptHandler(_context));
            // .SetNext(new FetchReleaseInfoHandler())
            // .SetNext(new DownloadReleaseHandler())
            // .SetNext(new UnzipHandler())
            // .SetNext(new DockerComposeHandler())
            // .SetNext(new RunMigrationsHandler());

        return rootHandler;
    }
    
    public IHandler BuildUpdateChain()
    {
        var rootHandler = new CheckForExistingInstallationHandler(_context);

        // rootHandler
        //     .SetNext(new FetchReleaseInfoHandler())
        //     .SetNext(new DownloadReleaseHandler())
        //     .SetNext(new UnzipHandler())
        //     .SetNext(new DockerComposeHandler())
        //     .SetNext(new RunMigrationsHandler());

        return rootHandler;
    }
}
