using Launcher.Handlers;
using Launcher.Services;

namespace Launcher.Menu;

public class UpdateMenuItem : MenuItem, IHiddenMenuItem
{
    public override async Task<HandlerResult> OnSelectedAsync()
    {
        var updateChain = new ChainBuilder().BuildUpdateChain();
        var handlerResult = await new ChainExecutor().Execute(updateChain);
        return handlerResult;
    }
    
    public bool IsHidden { get; } = false;
    public UpdateMenuItem(string newVersion) : base($"[chartreuse1][[U]][/]pdate to v{newVersion}", MenuAction.Update)
    {
    }
}
