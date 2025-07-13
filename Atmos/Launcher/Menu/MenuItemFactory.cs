using Launcher.Models;
using Launcher.Services;

namespace Launcher.Menu;

public class MenuItemFactory
{
    private readonly LauncherContext _context;
    private readonly ChainBuilder _builder;
    public MenuItemFactory(ChainBuilder builder, LauncherContext context)
    {
        _builder = builder;
        _context = context;
    }
    
    public List<MenuItem> GetMenuItems()
    {
        return [
            new OpenDashboardMenuItem(),
            new ToggleServiceMenuItem(true), // Example value, this should be dynamically set based on service status
            new RestartServiceMenuItem(),
            new UpdateMenuItem(_context, _builder),
            new LogsMenuItem(),
            new ExitMenuItem()
        ];
    }
}
