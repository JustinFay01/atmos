using System.Diagnostics;

using Launcher.Handlers.Abstract;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class RunMigrationsHandler : DefaultSetNextHandler
{
    public override string StepName => "Updating database";
    private const string MigrationExe = "app/atmos-migrate";
    public override async Task<HandlerResult> HandleAsync(InstallationContext context, ExecutorOptions? options = null)
    {
        var fullMigrationPath = Path.Combine(context.Config.InstallPath, MigrationExe);
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

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if ((string.IsNullOrWhiteSpace(output) || options?.DebugMode != true) && process.ExitCode == 0)
            {
                return process.ExitCode != 0
                    ? HandlerResult.Failure($"Migration failed. Are you sure the database is running?")
                    : HandlerResult.Success("Database migration completed successfully.");
            }

            AnsiConsole.MarkupInterpolated($"[white]Migration Output:{output}[/]");
            return process.ExitCode != 0 
                ? HandlerResult.Failure("Migration failed. Are you sure the database is running?") 
                : HandlerResult.Success("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            return HandlerResult.Failure($"An error occurred during migration: {ex.Message}");
        }
    }
}
