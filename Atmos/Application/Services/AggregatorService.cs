using System.Collections.Concurrent;

using Application.Interfaces;

using Domain.Entities;

using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AggregatorService : IAggregator
{
    private readonly ILogger<AggregatorService> _logger;

    public AggregatorService(ILogger<AggregatorService> logger)
    {
        _logger = logger;
    }

    public ConcurrentQueue<Reading> TenSecondReadings { get; } = [];
    public ConcurrentQueue<Reading> OneMinuteRollingAverage { get; } = [];
    public ConcurrentQueue<Reading> FiveMinuteRollingAverage { get; } = [];

    public Task ProcessReadingAsync(Reading reading)
    {
        throw new NotImplementedException();
    }
}
