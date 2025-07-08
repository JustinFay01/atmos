using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Hardware;

public class MockSensorClient : ISensorClient
{
    public bool IsConnected { get; } = true;

    private int _iterationCount = 0;
    private readonly Random _random = new Random();
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return true;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<RawSensorReading> GetReadingAsync(CancellationToken cancellationToken)
    {
        _iterationCount++;
        var reading = RealisticReading();
        return Task.FromResult(reading);
    }

    private RawSensorReading RealisticReading()
    {
        return new RawSensorReading
        {
            Temperature = 68 + _random.Next(-5, 5) + _iterationCount % 10,
            Humidity = 50 + _random.Next(-10, 10) + _iterationCount % 5,
            DewPoint = 10 + _random.Next(-2, 2) + _iterationCount % 3,
        };
    }

    private RawSensorReading MonotonicallyIncreasingReading()
    {
        return new RawSensorReading
        {
            Temperature = _iterationCount,
            Humidity = _iterationCount * 2,
            DewPoint = _iterationCount * 3,
        };
    }
}
