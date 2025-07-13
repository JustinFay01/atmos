using Launcher.Models;

namespace Launcher.Handlers.Abstract;

public abstract class DefaultSetNextHandler : IHandler
{
    public virtual IHandler? Next { get; protected set; }
    public abstract string StepName { get; }

    protected LauncherContext Context;
    protected DefaultSetNextHandler(LauncherContext context)
    {
        Context = context;
    }
    public abstract Task<HandlerResult> HandleAsync();
    
    public virtual IHandler SetNext(IHandler handler)
    {
        Next = handler;
        return Next;
    }
}
