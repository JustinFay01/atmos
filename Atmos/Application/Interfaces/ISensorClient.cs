using Application.Models;

namespace Application.Interfaces;

public interface ISensorClient
{
    public bool IsConnected { get; }
    
    /// <summary>
    /// Try to connect to the sensor client.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Task that represents the asynchronous operation. The task result contains true if connected, false otherwise.</returns>
    public Task<bool> ConnectAsync(CancellationToken cancellationToken);
    public Task DisconnectAsync(CancellationToken cancellationToken);
    public Task<RawSensorReading> GetReadingAsync(CancellationToken cancellationToken);

}
