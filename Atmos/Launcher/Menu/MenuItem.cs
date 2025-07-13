using Launcher.Handlers;

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

public class LogsMenuItem : MenuItem
{
    public override Task<HandlerResult> OnSelectedAsync()
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    
    public LogsMenuItem() : base("[grey][[L]][/]ogs", MenuAction.Logs) { }
}

public class ExitMenuItem : MenuItem
{
    public override Task<HandlerResult> OnSelectedAsync()
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    
    public ExitMenuItem() : base("[red][[E]][/]xit", MenuAction.Exit) { }
}
