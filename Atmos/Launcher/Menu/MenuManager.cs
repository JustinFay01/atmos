using Microsoft.Extensions.Hosting;

using Spectre.Console;

namespace Launcher.Menu;

public enum MenuAction
{
    OpenDashboard,
    ToggleService,
    RestartService,
    Update,
    Logs,
    Exit
}

public class MenuManager : BackgroundService
{
    private readonly MenuItemFactory _menuItemFactory;
    
    public MenuManager(MenuItemFactory menuItemFactory)
    {
        _menuItemFactory = menuItemFactory;
    }
    
    /// <summary>
    /// Check if Atmos is installed by looking for the AtmosConfig file.
    /// If it is, load the configuration, check for updates, and show the menu.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }
    
    /// <summary>
    /// Orchestrates the menu display and handles user input.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var menuItem = await ShowMenuAsync(stoppingToken);
        await menuItem.OnSelectedAsync();
    }
    
    private async Task<MenuItem> ShowMenuAsync(CancellationToken cancellationToken = default)
    {
        var prompt = new SelectionPrompt<MenuItem>()
            .Title("\n[bold cyan]Select an option:[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
            .UseConverter(item => item.DisplayText) // Show our formatted text in the list
            .AddChoices(_menuItemFactory.GetMenuItems().Where(item => item is not IHiddenMenuItem { IsHidden: true }));
        
        return await AnsiConsole.PromptAsync(prompt, cancellationToken);
    }

    
}
