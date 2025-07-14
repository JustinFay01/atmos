using Launcher.Models;
using Launcher.Services;

using Microsoft.Extensions.Hosting;

namespace Launcher.Menu;

public class MenuItemFactory
{
    private readonly LauncherContext _context;
    private readonly ChainBuilder _builder;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IAtmosLogService _logService;
    public MenuItemFactory(ChainBuilder builder, LauncherContext context, IHostApplicationLifetime appLifetime, IAtmosLogService logService)
    {
        _builder = builder;
        _context = context;
        _appLifetime = appLifetime;
        _logService = logService;
    }
    
    public List<MenuItem> GetMenuItems()
    {
        return [
            new OpenDashboardMenuItem(),
            new ToggleServiceMenuItem(true), // Example value, this should be dynamically set based on service status
            new RestartServiceMenuItem(),
            new UpdateMenuItem(_context, _builder),
            new LogsMenuItem(_logService),
            new ExitMenuItem(_appLifetime)
        ];
    }
}
