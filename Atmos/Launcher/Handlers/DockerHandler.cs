using System.Diagnostics;

namespace Launcher.Handlers;

public class DockerHandler : IInstallationHandler
{
    public string StepName => "Check Docker Installation";
    private IInstallationHandler? _nextHandler;

    public IInstallationHandler SetNext(IInstallationHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public async Task<HandlerResult> HandleAsync(InstallationContext context)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        
        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            const string errorMessage = "Docker check failed. Please ensure Docker Desktop is installed and running.";
            return new HandlerResult(false, errorMessage);
        }

        if (_nextHandler != null)
        {
            return await _nextHandler.HandleAsync(context);
        }

        return new HandlerResult(true, "Docker check successful.");
    }
}
