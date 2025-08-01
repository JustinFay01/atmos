using Launcher.Models;
using Launcher.Services;


public record HandlerResult(bool IsSuccess, string Message, int ExitCode = 0)
{
        
    public static HandlerResult Success(string message)
    {
        return new HandlerResult(true, message);
    }
    
    public static HandlerResult Failure(string message, int exitCode = 1)
    {
        return new HandlerResult(false, message, exitCode);
    }
}

public interface IHandler
{
    /// <summary>
    /// A descriptive name for the step, used for UI feedback.
    /// </summary>
    string StepName { get; }

    /// <summary>
    /// Sets the next handler in the chain.
    /// </summary>
    IHandler SetNext(IHandler handler);
    
    IHandler? Next { get; }

    /// <summary>
    /// Executes the logic for this handler.
    /// </summary>
    Task<HandlerResult> HandleAsync(CancellationToken cancellationToken = default);
}

public interface IInteractiveHandler
{
 
}
