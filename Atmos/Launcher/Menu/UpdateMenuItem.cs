using System.Collections.Frozen;

using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Menu;

public class UpdateMenuItem : MenuItem, IHiddenMenuItem
{
    public bool IsHidden => !_context.NewUpdateAvailable; 
    public override FrozenSet<ConsoleKey> Keys => [ConsoleKey.U];
    
    private readonly LauncherContext _context;
    private readonly ChainBuilder _builder;
    
    public UpdateMenuItem(LauncherContext context, ChainBuilder builder) : base($"[chartreuse1][[U]][/]pdate to {context.FetchedVersionTag}", MenuAction.Update)
    {
        _context = context;
        _builder = builder;
    }
    public override async Task<HandlerResult> OnSelectedAsync(CancellationToken cancellationToken = default)
    {
        var updateChain = _builder.BuildChain(ChainType.Update);
        var handlerResult = await new ChainExecutor().ExecuteInstallation(updateChain, cancellationToken);
        return handlerResult;
    }
}
