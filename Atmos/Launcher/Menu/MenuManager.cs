using System.Diagnostics;
using System.Text;

using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;
using Launcher.Util;

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
    private readonly IAtmosLogService _logService;
    private readonly LauncherContext _context;
    
    public MenuManager(MenuItemFactory menuItemFactory, IHostApplicationLifetime appLifetime, ChainBuilder builder, IAtmosLogService logService, LauncherContext context)
    {
        _menuItemFactory = menuItemFactory;
        _appLifetime = appLifetime;
        _builder = builder;
        _logService = logService;
        _context = context;
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
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
                AnsiConsole.Clear();
                WriteStatusPanel();
                try
                {
                    var menuItem = await ShowMenuAsync(timeoutTokenSource.Token);
                    await menuItem.OnSelectedAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
            }
        } catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[bold red]Menu operation was cancelled.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            AnsiConsole.MarkupLine("[bold red]An unexpected error occurred in the menu manager.[/]");
        }
        finally
        {
            _appLifetime.StopApplication();
        }
     
    }

    private void WriteStatusPanel()
    {
        var isRunning = _context.RunningProcesses.ContainsKey(ProcessKey.AtmosApi) && !_context.RunningProcesses[ProcessKey.AtmosApi].HasExited;
        var statusMessage = isRunning 
            ? "[green]Atmos is running.[/]" 
            : "[red]Atmos is not running.[/]";
        var panelMessage = new Markup($"[yellow]Version:[/] {_context.FetchedVersionTag}\n" +
                                      $"[yellow]Atmos Status:[/] {statusMessage}");
        var panel = new Panel(panelMessage)
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("Atmos Status"),
            Expand = true
        };
        AnsiConsole.Write(panel);
    }
    
    private async Task<MenuItem> ShowMenuAsync(CancellationToken cancellationToken = default)
    {
        var prompt = new SelectionPrompt<MenuItem>()
            .Title("\n[bold cyan]Select an option:[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
            .UseConverter(item => item.DisplayText) // Show our formatted text in the list
            .AddChoices(_menuItemFactory.MenuItems.Where(item => item is not IHiddenMenuItem { IsHidden: true }));
        
        return await AnsiConsole.PromptAsync(prompt, cancellationToken);
    }
}
