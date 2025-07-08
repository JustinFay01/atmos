using Application.Dtos;
using Application.Helper;
using Application.Interfaces;
using Application.Models;

using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Responsible for storing a cache of, and sending updates for, the last 12 hours of readings.
/// </summary>
public interface IHourlyReadingService
{
    public IList<HourReadingDto?> HourlyReadings { get; }
    
    public Task ProcessReadingAsync(RawSensorReading reading, CancellationToken cancellationToken = default);

}

public class HourlyReadingService(ILogger<HourlyReadingService> logger, IRealtimeUpdateNotifier realtimeUpdateNotifier)
    : IHourlyReadingService
{
    private const int MaxReadings = 24;

    public IList<HourReadingDto?> HourlyReadings { get; } = new HourReadingDto[MaxReadings];

    public async Task ProcessReadingAsync(RawSensorReading reading, CancellationToken cancellationToken = default)
    {
        if (reading is null)
        {
            throw new ArgumentNullException(nameof(reading), "Reading cannot be null.");
        }

        var currentHour = DateTimeProvider.Instance.Now.Hour;
        var test = HourlyReadings[currentHour];
        // Update the reading for the current hour if it is null or if the timestamp
        if (HourlyReadings[currentHour] is null || HourlyReadings[currentHour]?.Timestamp.Day != reading.Timestamp.Day)
        {
            logger.LogInformation("New hour detected: {Hour}. Processing reading: {Reading}", 
                DateTimeProvider.Instance.Now.Hour, reading);
        
            var newReading = new HourReadingDto
            {
                Hour = reading.Timestamp.Hour,
                Timestamp = reading.Timestamp,
                Temperature = reading.Temperature,
                Humidity = reading.Humidity,
                DewPoint = reading.DewPoint
            };
        
            HourlyReadings[currentHour] = newReading;
        
            await realtimeUpdateNotifier.SendHourUpdateAsync(HourlyReadings, cancellationToken);
        }
    }
}
