namespace Application.Models;

public record SensorSettings
{
    /// <summary>
    /// Clock drift tolerance in millisec2onds.
    /// </summary>
    public long Tolerance { get; init; }
    
    /// <summary>
    /// Start readings at the top of the minute (:00)
    /// </summary>
    public bool WaitTillNearestTenSeconds { get; init; }
    
    /// <summary>
    /// Delay cushion adjusts how long WaitForNextTickAsync waits before processing the next sensor reading.
    /// This prevents rollback from :00 to :59, which can happen if tick is slightly early.
    /// </summary>
    public int DelayCushion { get; init; }
    
    /// <summary>
    /// Number of attempts to connect to the sensor client before giving up.
    /// </summary>
    public int MaxRetryCount { get; init; }
}
