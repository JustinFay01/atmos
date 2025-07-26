using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Launcher.Models;
using Launcher.Util;

using Microsoft.Extensions.Hosting;

namespace Launcher.Menu;

public interface IHiddenMenuItem
{
    bool IsHidden { get; }
}

public abstract class MenuItem
{
    public string DisplayText { get; }
    public MenuAction Action { get; }
    public abstract FrozenSet<ConsoleKey> Keys { get; }

    public abstract Task<HandlerResult> OnSelectedAsync(CancellationToken cancellationToken = default);

    protected MenuItem(string displayText, MenuAction action)
    {
        DisplayText = displayText;
        Action = action;
    }
}

public class OpenDashboardMenuItem : MenuItem
{
    private readonly LauncherContext _context;
    public override FrozenSet<ConsoleKey> Keys => [ConsoleKey.O];
    public override Task<HandlerResult> OnSelectedAsync(CancellationToken cancellationToken = default)
    {
        if (_context.RunningProcesses.TryGetValue(ProcessKey.AtmosDashboard, out var value) && !value.HasExited)
        {
            return Task.FromResult(HandlerResult.Success("Atmos Dashboard is already running."));
        }

        try
        {
            _context.RunningProcesses[ProcessKey.AtmosDashboard] = Process.Start(_context.DashboardUrl);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var tempUrl = _context.DashboardUrl.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {tempUrl}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", _context.DashboardUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", _context.DashboardUrl);
            }
            else
            {
                throw;
            }
        }
        return Task.FromResult(HandlerResult.Success("Atmos Dashboard is running."));
    }
    public OpenDashboardMenuItem(LauncherContext context) : base("[aqua][[O]][/]pen Dashboard", MenuAction.OpenDashboard)
    {
        _context = context;
    }
}

public class RestartServiceMenuItem : MenuItem
{
    public override FrozenSet<ConsoleKey> Keys => [ConsoleKey.R];
    public override Task<HandlerResult> OnSelectedAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HandlerResult.Success("Exiting..."));
    }
    public RestartServiceMenuItem() : base("[yellow][[R]][/]estart Service", MenuAction.RestartService) { }
}

public class ExitMenuItem : MenuItem
{
    public override FrozenSet<ConsoleKey> Keys => [ConsoleKey.E, ConsoleKey.Escape];
    
    private readonly IHostApplicationLifetime _lifetime;
    public ExitMenuItem(IHostApplicationLifetime lifetime) : base("[red][[E]][/]xit", MenuAction.Exit)
    {
        _lifetime = lifetime;
    }
    
    public override Task<HandlerResult> OnSelectedAsync(CancellationToken cancellationToken = default)
    {
        _lifetime.StopApplication();
        return Task.FromResult(HandlerResult.Success("Exiting Atmos..."));
    }
}
