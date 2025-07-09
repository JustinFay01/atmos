namespace Launcher.Handlers;

public record HandlerResult(bool IsSuccess, string Message);

public interface IInstallationHandler
{
    /// <summary>
    /// A descriptive name for the step, used for UI feedback.
    /// </summary>
    string StepName { get; }

    /// <summary>
    /// Sets the next handler in the chain.
    /// </summary>
    IInstallationHandler SetNext(IInstallationHandler handler);

    /// <summary>
    /// Executes the logic for this handler.
    /// </summary>
    Task<HandlerResult> HandleAsync(InstallationContext context);
}
