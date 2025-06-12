using System.Collections.Concurrent;

using Domain.Entities;

namespace Application.Interfaces;

public interface IAggregator
{
    public IReadOnlyCollection<Reading> TenSecondReadings { get; }

    public IReadOnlyCollection<Reading> OneMinuteRollingAverage { get; }

    public IReadOnlyCollection<Reading> FiveMinuteRollingAverage { get; }

    public Task ProcessReadingAsync(Reading reading, CancellationToken cancellationToken = default);
}
