using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Hardware;

public class MockSensorClient : ISensorClient
{
    public bool IsConnected { get; } = true;

    private int _iterationCount = 0;
    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<RawSensorReading> GetReadingAsync(CancellationToken cancellationToken)
    {
        _iterationCount++;
        var reading = new RawSensorReading
        {
            Temperature = _iterationCount,
            Humidity = _iterationCount * 2,
            DewPoint = _iterationCount * 3,
        };

        return Task.FromResult(reading);
    }
}
