using Application.Interfaces;

using Domain.Entities;

namespace Application.Services;

public class AggregatorService : IAggregator
{
    private readonly ILogger<AggregatorService> _logger;

    public AggregatorService(ILogger<AggregatorService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyCollection<Reading> TenSecondReadings { get; }
    public IReadOnlyCollection<Reading> OneMinuteRollingAverage { get; }
    public IReadOnlyCollection<Reading> FiveMinuteRollingAverage { get; }

    public Task ProcessReadingAsync(Reading reading)
    {
        throw new NotImplementedException();
    }
}
