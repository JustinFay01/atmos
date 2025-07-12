namespace Launcher.Handlers.Abstract;

public abstract class DefaultSetNextHandler : IInstallationHandler
{
    public virtual IInstallationHandler? Next { get; protected set; }
    public virtual IInstallationHandler SetNext(IInstallationHandler handler)
    {
        Next = handler;
        return Next;
    }
    
    
    public abstract string StepName { get; }
    public abstract Task<HandlerResult> HandleAsync(InstallationContext context);
}
