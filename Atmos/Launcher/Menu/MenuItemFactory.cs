namespace Launcher.Menu;

public class MenuItemFactory
{

    public MenuItemFactory()
    {
        
    }
    
    public List<MenuItem> GetMenuItems()
    {
        return [
            new OpenDashboardMenuItem(),
            new ToggleServiceMenuItem(true), // Example value, this should be dynamically set based on service status
            new RestartServiceMenuItem(),
            new UpdateMenuItem("1.0.0"), // Example version, this should be dynamically set
            new LogsMenuItem(),
            new ExitMenuItem()
        ];
    }
}
