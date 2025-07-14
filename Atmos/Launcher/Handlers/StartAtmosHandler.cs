using System.Diagnostics;

using Launcher.Handlers.Abstract;
using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

using Spectre.Console;

namespace Launcher.Handlers;

[HandlerOrder(ChainType.Initialization, 40)]
public class StartAtmosHandler : DefaultSetNextHandler
{
    public override string StepName => "Starting Atmos";
    private const string AtmosExe = "app/atmos";
    private const string DebugExe = "/Users/justin.fay/code/personal/atmos/release-zips/1.0.9-alpha/app/atmos";
    private readonly IAtmosLogService _logService;
    
    public StartAtmosHandler(LauncherContext context, IAtmosLogService logService) : base(context)
    {
        _logService = logService;
    }
    
    public override Task<HandlerResult> HandleAsync()
    {
        // var fullAtmosPath = Path.Combine(Context.Config.InstallPath, AtmosExe);
        // if (!File.Exists(fullAtmosPath))
        // {
        //     return Task.FromResult(HandlerResult.Failure("Atmos executable not found. Please ensure the Atmos installation is complete."));
        // }
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = DebugExe,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

             var process = Process.Start(processStartInfo);

             if (process == null)
             {
                 return Task.FromResult(HandlerResult.Failure("Failed to start Atmos process."));
             }
             
             process.OutputDataReceived += (sender, args) => 
             {
                 if (args.Data != null)
                 {
                     _logService.AddLog(args.Data);
                 }
             };
             
             Context.RunningProcesses["atmos"] = process;
            
            return Task.FromResult(HandlerResult.Success("Atmos started successfully."));
        }
        catch (Exception ex)
        {
            if (Context.DebugMode)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
            return Task.FromResult(HandlerResult.Failure($"An error occurred while starting Atmos: {ex.Message}"));
        }
    }
}
