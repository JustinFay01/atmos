using Application.DTOs;
using Application.Interfaces;

namespace Infrastructure.Hardware;

public class MockSensorClient : ISensorClient
{
    public bool IsConnected { get; } = true;
    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ReadingDto> GetReadingAsync(CancellationToken cancellationToken)
    {
        // Simulate a sensor reading
        var reading = new ReadingDto
        {
            Temperature = 22.5f,
            Humidity = 45.0f,
            DewPoint = 18.0f,
        };

        return Task.FromResult(reading);
    }
}
