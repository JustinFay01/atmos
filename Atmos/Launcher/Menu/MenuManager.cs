using Launcher.Handlers.Attributes;
using Launcher.Services;

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
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ChainBuilder _builder;
    
    public MenuManager(MenuItemFactory menuItemFactory, IHostApplicationLifetime appLifetime, ChainBuilder builder)
    {
        _menuItemFactory = menuItemFactory;
        _appLifetime = appLifetime;
        _builder = builder;
    }
    
    /// <summary>
    /// Check if Atmos is installed by looking for the AtmosConfig file.
    /// If it is, load the configuration, check for updates, and show the menu.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
       var startupChain = _builder.BuildChain(ChainType.Initialization);
       await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[cyan]Starting Atmos...[/]", async ctx =>
            { 
               var startupResult = await new ChainExecutor().ExecuteSilentChainAsync(startupChain, cancellationToken);
                if (!startupResult.IsSuccess)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {startupResult.Message}[/]");
                    AnsiConsole.MarkupLine("[bold red]Atmos could not start. Please check the logs for more details.[/]");
                    _appLifetime.StopApplication();
                }
            });
       
        await base.StartAsync(cancellationToken);
    }
    
    /// <summary>
    /// Orchestrates the menu display and handles user input.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            var menuItem = await ShowMenuAsync(stoppingToken);
            await menuItem.OnSelectedAsync();
            var continueSelection = await AskToContinueAsync(stoppingToken);
            if (continueSelection)
            {
                continue;
            }

            AnsiConsole.MarkupLine("[bold green]Thank you for using Atmos![/]");
            _appLifetime.StopApplication();
            break;
        }
    }
    
    private async Task<bool> AskToContinueAsync(CancellationToken cancellationToken = default)
    {
        var prompt = new ConfirmationPrompt("[bold yellow]Do you want to continue?[/]");
        return await AnsiConsole.PromptAsync(prompt, cancellationToken);
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
