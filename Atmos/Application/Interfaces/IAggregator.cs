using System.Collections.Concurrent;

using Domain.Entities;

namespace Application.Interfaces;

public interface IAggregator
{
    public ConcurrentQueue<Reading> TenSecondReadings { get; }

    public ConcurrentQueue<Reading> OneMinuteRollingAverage { get; }

    public ConcurrentQueue<Reading> FiveMinuteRollingAverage { get; }

    public Task ProcessReadingAsync(Reading reading, CancellationToken cancellationToken = default);
}
