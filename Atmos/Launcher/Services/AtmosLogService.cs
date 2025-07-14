namespace Launcher.Services;

public interface IAtmosLogService
{
    public event Action<string>? OnLogReceived;
    public void AddLog(string message);
     public void Clear();
     public IReadOnlyList<string> GetAllLogs();
}

public class AtmosLogService : IAtmosLogService
{
    private readonly List<string> _logs = [];
    private readonly Lock _lock = new();
    private const int MaxLogCount = 100; 

    public event Action<string>? OnLogReceived;

    // This method is now synchronous (void), fast, and thread-safe.
    // It is compatible with event handlers like OutputDataReceived.
    public void AddLog(string message)
    {
        lock (_lock)
        {
            _logs.Add(message);
            if (_logs.Count > MaxLogCount)
            {
                _logs.RemoveAt(0);
            }
        }
        OnLogReceived?.Invoke(message);
    }
    
    // Returns a *copy* of the list, making it safe for the caller
    // to iterate over even if the original list is modified.
    public IReadOnlyList<string> GetAllLogs()
    {
        lock (_lock)
        {
            return _logs.ToList(); 
        }
    }
    
    public void Clear()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
        OnLogReceived?.Invoke("--- Logs Cleared ---");
    }
}
