using System.Text;

using Launcher.Services;

using Spectre.Console;

namespace Launcher.Menu;

public class LogsMenuItem : MenuItem
{
    private readonly IAtmosLogService _logService;
    private readonly List<string> errors = [];
    private readonly List<string> restOfLogs = [];
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
        var allLogs = _logService.GetAllLogs();
        foreach (var log in allLogs.TakeLast(100))
        {
            // Basic color coding example
            if (log.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Markup.Escape(log));
            }
            else {
                restOfLogs.Add(Markup.Escape(log));
            }
        }
    }

    private Task ShowLogsViewAsync(CancellationToken cancellationToken = default)
    {
        // This event tells our live view when to redraw because new data has arrived.
        var refreshLogsEvent = new ManualResetEventSlim(false);

        _logService.OnLogReceived += OnLogReceived;
        var optionsPanel = new Panel(new Markup("Actions: [blue](B)[/]ack | [yellow](C)[/]lear")
            )
            .Header("[bold]Logs[/]")
            .Expand();
        AnsiConsole.Write(optionsPanel);
        var table = new Table().Centered()
            .ShowHeaders()
            .Expand();
        var errorsColumn = new TableColumn("Errors").LeftAligned();
        var logsColumn = new TableColumn("Logs")
            .LeftAligned();
                    
        table.AddColumn(logsColumn);
        table.AddColumn(errorsColumn);

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Start(ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ProcessLogs();
                    table.Rows.Clear();
                    
                    // 1. Populate the table with the latest logs
                    foreach (var error in errors)
                    {
                        table.AddRow(new Markup(""),new Markup($"[red]{error}[/]"));
                    }
                    foreach (var log in restOfLogs)
                    {
                        table.AddRow(new Markup(log),new Markup(""));
                    }

                    // 2. Update the live display
                    ctx.UpdateTarget(table);
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
                        errors.Clear();
                        restOfLogs.Clear();
                    }
                }
            });

        return Task.CompletedTask;
        
        void OnLogReceived(string log) => refreshLogsEvent.Set();
    }
}
