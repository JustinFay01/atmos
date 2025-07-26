using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Install, 70)]
[HandlerOrder(ChainType.Update, 60)]
public class RunMigrationsHandler : DefaultSetNextHandler
{
    public RunMigrationsHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Updating database";
    private readonly string _migrationExe = Path.Combine("app", "atmos-migrate");
    public override async Task<HandlerResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        var fullMigrationPath = Path.Combine(Context.Config.InstallPath, _migrationExe);
        if (!File.Exists(fullMigrationPath))
        {
            return HandlerResult.Failure("Migration executable not found. Please ensure the Atmos installation is complete.");
        }
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fullMigrationPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                return HandlerResult.Failure("Failed to start migration process.");
            }

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            if ((string.IsNullOrWhiteSpace(output) && Context.DebugMode)|| process.ExitCode != 0)
            {
                AnsiConsole.MarkupInterpolated($"[white]Migration Output:{output}[/]");
            }
            
            return process.ExitCode != 0
                ? HandlerResult.Failure($"Migration failed. Are you sure the database is running?")
                : HandlerResult.Success("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            return HandlerResult.Failure($"An error occurred during migration: {ex.Message}");
        }
    }
}
