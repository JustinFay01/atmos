using System.Text;

using Launcher.Services;

using Spectre.Console;

namespace Launcher.Menu;

public class LogsMenuItem : MenuItem
{
    private readonly IAtmosLogService _logService;
    private readonly StringBuilder _logContent = new();
    public override async Task<HandlerResult> OnSelectedAsync()
    {
        var cts = new CancellationTokenSource();
        await ShowLogsViewAsync(cts.Token);
        return HandlerResult.Success("Logs view closed.");
    }
    
    public LogsMenuItem(IAtmosLogService logService) : base("[grey][[L]][/]ogs", MenuAction.Logs)
    {
        _logService = logService;
    }

    private void ProcessLogs()
    {
        // 1. Generate the content for the panel
        var allLogs = _logService.GetAllLogs();
        foreach (var log in allLogs.TakeLast(20)) // Show last 20 lines
        {
            // Basic color coding example
            if (log.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                _logContent.AppendLine(Markup.Escape($"[red]{log}[/]"));
            }
            else if (log.Contains("SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                _logContent.AppendLine(Markup.Escape($"[green]{log}[/]"));
            }
            else
            {
                _logContent.AppendLine(Markup.Escape(log));
            }
        }
    }

    private Task ShowLogsViewAsync(CancellationToken cancellationToken = default)
    {
        // This event tells our live view when to redraw because new data has arrived.
        var refreshLogsEvent = new ManualResetEventSlim(false);

        _logService.OnLogReceived += OnLogReceived;

        AnsiConsole.Live(new Panel(string.Empty))
            .Start(ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ProcessLogs();

                    var panelContent = new Rows(
                        new Panel(new Markup(_logContent.ToString()))
                            .Header("Streaming Logs")
                            .Expand(),
                        new Markup("[grey]Actions: [blue](B)[/]ack | [yellow](C)[/]lear[/]")
                    );

                    // 2. Update the live display
                    ctx.UpdateTarget(panelContent);
                    ctx.Refresh(); // Force an initial refresh

                    // 3. Wait for new logs OR user input (non-blocking)
                    // Wait for a signal for 100ms. If no signal, loop to check for key press.
                    refreshLogsEvent.Wait(100);
                    refreshLogsEvent.Reset();

                    if (!Console.KeyAvailable)
                    {
                        continue;
                    }

                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.B)
                    {
                        break;
                    }

                    if (key == ConsoleKey.C)
                    {
                        _logService.Clear();
                    }
                }
            });

        return Task.CompletedTask;
        
        void OnLogReceived(string log) => refreshLogsEvent.Set();
    }
}
