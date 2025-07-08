using Spectre.Console;

namespace Launcher;

internal abstract class Program
{
    // --- Stubs for State Simulation ---
    // In the final app, these values will be determined by checking the actual system.
    // For now, you can change them here to test the menu's appearance.
    private static bool _isServiceRunning = true;
    private static bool _isDatabaseOnline = true;
    private static string _currentVersion = "1.0.0";
    // Set this to a version string (e.g., "1.1.0") to test the "Update Available" UI.
    // Set it to null to hide the update option.
    private static string? _availableUpdateVersion = "1.1.0"; 
    
    public static void Main(string[] args)
    {
        // TODO: Add logic here to detect if this is a first-time install.
        // For now, we will assume it's already installed and jump to the management menu.
        
        while (true)
        {
            Console.Clear();
            ShowMainMenu();
        }
    }

    private static void ShowMainMenu()
    {
        // Use Spectre.Console's markup for colors and styles
        AnsiConsole.MarkupLine("[underline bold blue]Atmos Control Panel[/]");
        AnsiConsole.WriteLine(); // Blank line for spacing

        // Display the status panel
        var statusPanel = new Panel(
            $"[bold]Version:[/]         {_currentVersion}\n" +
            $"[bold]Service Status:[/]  {(_isServiceRunning ? "[green]Running[/]" : "[red]Stopped[/]")}\n" +
            $"[bold]Database Status:[/] {(_isDatabaseOnline ? "[green]Online[/]" : "[red]Offline[/]")}"
        )
        {
            Header = new PanelHeader("[yellow]System Status[/]"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1)
        };
        AnsiConsole.Write(statusPanel);
        
        // Use a SelectionPrompt for the menu
        var prompt = new SelectionPrompt<string>()
            .Title("\n[bold]What would you like to do?[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more options)[/]");

        // --- Add Menu Choices ---

        // The "Start/Stop" option is dynamic based on the service state
        prompt.AddChoice(_isServiceRunning ? "Stop Service" : "Start Service");

        prompt.AddChoices(new[] {
            "Restart Service",
            "Open Dashboard",
            "View Logs"
        });

        // The "Update" option is conditional
        if (_availableUpdateVersion != null)
        {
            // Add the choice with a special color to draw attention
            prompt.AddChoice($"[bold yellow]Update to v{_availableUpdateVersion}[/]");
        }

        prompt.AddChoice("Exit");

        // Show the prompt and get the user's choice
        var choice = AnsiConsole.Prompt(prompt);
        
        // Handle the user's choice
        HandleMenuChoice(choice);
    }

    private static void HandleMenuChoice(string choice)
    {
        // We use a switch statement to handle the action.
        // The "[bold yellow]...[/]" markup is stripped before comparison.
        switch (Markup.Remove(choice))
        {
            case "Start Service":
                AnsiConsole.MarkupLine("[yellow]Starting service...[/]");
                // TODO: Wire up "sc.exe start AtmosService"
                _isServiceRunning = true; // Simulate success
                break;

            case "Stop Service":
                AnsiConsole.MarkupLine("[yellow]Stopping service...[/]");
                // TODO: Wire up "sc.exe stop AtmosService"
                _isServiceRunning = false; // Simulate success
                break;

            case "Restart Service":
                AnsiConsole.MarkupLine("[yellow]Restarting service...[/]");
                // TODO: Wire up stop and start logic
                _isServiceRunning = true; // Simulate success
                break;
                
            case "Open Dashboard":
                AnsiConsole.MarkupLine("[green]Opening dashboard in your default browser...[/]");
                // TODO: Wire up Process.Start("http://localhost:5000") or your app's URL
                break;

            case "View Logs":
                AnsiConsole.MarkupLine("[grey]Displaying application logs...[/]");
                // TODO: Wire up logic to read and display the log file
                break;

            case { } s when s.StartsWith("Update to"):
                AnsiConsole.MarkupLine($"[yellow]Initiating update to version {_availableUpdateVersion}...[/]");
                // TODO: Wire up the entire update process from the roadmap
                break;
                
            case "Exit":
                AnsiConsole.MarkupLine("[bold]Goodbye![/]");
                Environment.Exit(0);
                break;
        }

        // Pause to allow the user to see the result before the menu refreshes
        AnsiConsole.WriteLine();
        AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Press Enter to continue...[/]")
            .AllowEmpty());
    }
}

