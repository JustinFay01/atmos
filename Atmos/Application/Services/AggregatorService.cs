using System.Collections.Concurrent;

using Application.DTOs;
using Application.Interfaces;

using Domain.Entities;

using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AggregatorService : IAggregator
{
    private readonly ILogger<AggregatorService> _logger;
    private readonly IRealtimeUpdateNotifier _notifier;

    public AggregatorService(ILogger<AggregatorService> logger, IRealtimeUpdateNotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
        
        TenSecondReadings = new ConcurrentQueue<Reading>();
        OneMinuteRollingAverage = new ConcurrentQueue<Reading>();
        FiveMinuteRollingAverage = new ConcurrentQueue<Reading>();
    }

    public IReadOnlyCollection<Reading> TenSecondReadings { get; } 
    public IReadOnlyCollection<Reading> OneMinuteRollingAverage { get; } 
    public IReadOnlyCollection<Reading> FiveMinuteRollingAverage { get; } 

    public Task ProcessReadingAsync(Reading reading, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Processing reading: {reading}", reading);

        _notifier.SendDashboardUpdate(new ReadingDto
        {
            Temperature = Random.Shared.Next(20, 30),
            Humidity = Random.Shared.Next(30, 60),
            DewPoint = Random.Shared.Next(1000, 1020),
        }, cancellationToken);

        return Task.CompletedTask;
    }
}
