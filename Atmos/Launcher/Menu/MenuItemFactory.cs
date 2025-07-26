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
    private readonly List<MenuItem> _menuItems = [];
    public List<MenuItem> MenuItems => _menuItems.Count > 0 ? _menuItems : GetMenuItems();

    public MenuItemFactory(ChainBuilder builder, LauncherContext context, IHostApplicationLifetime appLifetime, IAtmosLogService logService)
    {
        _builder = builder;
        _context = context;
        _appLifetime = appLifetime;
        _logService = logService;
    }
    
    private List<MenuItem> GetMenuItems()
    {
        return [
            new OpenDashboardMenuItem(_context),
            new RestartServiceMenuItem(),
            new UpdateMenuItem(_context, _builder),
            new LogsMenuItem(_logService),
            new ExitMenuItem(_appLifetime)
        ];
    }
}
