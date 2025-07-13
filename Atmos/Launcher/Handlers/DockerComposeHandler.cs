using System.Diagnostics;

using Launcher.Handlers.Abstract;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

public class DockerComposeHandler : DefaultSetNextHandler
{
    public override string StepName => "Setting up database with Docker Compose";
    public override async Task<HandlerResult> HandleAsync(InstallationContext context, ExecutorOptions? options = null)
    {
        var fullMigrationPath = Path.Combine(context.Config.InstallPath, "docker-compose.yml");
        if (!File.Exists(fullMigrationPath))
        {
            return HandlerResult.Failure("Docker Compose file not found. Please ensure the Atmos installation is complete.");
        }
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "compose up -d",
                WorkingDirectory = context.Config.InstallPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                return HandlerResult.Failure("Failed to start Docker Compose process.");
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var errorOutput = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if ((!string.IsNullOrWhiteSpace(output) && options?.DebugMode == true) || process.ExitCode != 0)
            {
                AnsiConsole.MarkupInterpolated($"[white]Docker Compose Output:{output}[/]");
            }
            if ((!string.IsNullOrWhiteSpace(output) && options?.DebugMode == true) || process.ExitCode != 0)
            {
                AnsiConsole.MarkupInterpolated($"[red]Docker Compose Error Output:{errorOutput}[/]");
            }
            return process.ExitCode != 0 
                ? HandlerResult.Failure("Docker Compose failed to set up the environment. Please check the output for errors.") 
                : HandlerResult.Success("Docker Compose setup completed successfully.");
        }
        catch (Exception ex)
        {
            return HandlerResult.Failure($"An error occurred during Docker Compose setup: {ex.Message}");
        }
    }
}
