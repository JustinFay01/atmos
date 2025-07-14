using Launcher.Handlers;

using Microsoft.Extensions.Hosting;

using Spectre.Console;

namespace Launcher.Menu;

public interface IHiddenMenuItem
{
    bool IsHidden { get; }
}

public abstract class MenuItem
{
    public string DisplayText { get; }
    public MenuAction Action { get; }

    //TODO: Add CancellationToken to OnSelectedAsync
    public abstract Task<HandlerResult> OnSelectedAsync();

    protected MenuItem(string displayText, MenuAction action)
    {
        DisplayText = displayText;
        Action = action;
    }
}

public class OpenDashboardMenuItem : MenuItem
{
    public override Task<HandlerResult> OnSelectedAsync()
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    public OpenDashboardMenuItem() : base("[aqua][[O]][/]pen Dashboard", MenuAction.OpenDashboard) { }
}

public class ToggleServiceMenuItem : MenuItem
{
    public override Task<HandlerResult> OnSelectedAsync()
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    public bool IsServiceRunning { get; set; }

    public ToggleServiceMenuItem(bool isServiceRunning) 
        : base(isServiceRunning ? "[orange1][[T]][/]oggle Service (Stop)" : "[green][[T]][/]oggle Service (Start)", MenuAction.ToggleService)
    {
        IsServiceRunning = isServiceRunning;
    }
}

public class RestartServiceMenuItem : MenuItem
{
    public override Task<HandlerResult> OnSelectedAsync()
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    public RestartServiceMenuItem() : base("[yellow][[R]][/]estart Service", MenuAction.RestartService) { }
}

public class ExitMenuItem : MenuItem
{

    private readonly IHostApplicationLifetime _lifetime;
    public ExitMenuItem(IHostApplicationLifetime lifetime) : base("[red][[E]][/]xit", MenuAction.Exit)
    {
        _lifetime = lifetime;
    }
    
    public override Task<HandlerResult> OnSelectedAsync()
    {
        _lifetime.StopApplication();
        return Task.FromResult(HandlerResult.Success("Exiting Atmos..."));
    }
}
