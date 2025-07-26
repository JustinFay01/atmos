using System.Diagnostics;

using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Install, 60)]
[HandlerOrder(ChainType.Update, 50)]
public class DockerComposeHandler : DefaultSetNextHandler
{
    public DockerComposeHandler(LauncherContext context) : base(context)
    {
    }

    public override string StepName => "Setting up database with Docker Compose";
    public override async Task<HandlerResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        var fullMigrationPath = Path.Combine(Context.Config.InstallPath, "docker-compose.yml");
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
                WorkingDirectory = Context.Config.InstallPath,
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

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorOutput = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            if ((!string.IsNullOrWhiteSpace(output) && Context.DebugMode) || process.ExitCode != 0)
            {
                AnsiConsole.MarkupInterpolated($"[white]Docker Compose Output:{output}[/]");
            }
            if ((!string.IsNullOrWhiteSpace(output) && Context.DebugMode) || process.ExitCode != 0)
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
