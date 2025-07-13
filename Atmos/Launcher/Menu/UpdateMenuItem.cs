using Launcher.Handlers.Attributes;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Menu;

public class UpdateMenuItem : MenuItem, IHiddenMenuItem
{
    public bool IsHidden { get; } = false;
    private readonly LauncherContext _context;
    private readonly ChainBuilder _builder;
    public UpdateMenuItem(LauncherContext context, ChainBuilder builder) : base($"[chartreuse1][[U]][/]pdate to {context.FetchedVersionTag}", MenuAction.Update)
    {
        _context = context;
        _builder = builder;
    }
    public override async Task<HandlerResult> OnSelectedAsync()
    {
        var updateChain = _builder.BuildChain(ChainType.Update);
        var handlerResult = await new ChainExecutor().Execute(updateChain);
        return handlerResult;
    }
}
