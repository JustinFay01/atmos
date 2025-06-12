using System.Collections.Concurrent;

using Application.Models;

namespace Application.Rules;

/// <summary>
/// Updates the recent readings queue with the latest value. Making sure it only keeps the last six readings.
/// </summary>
public class RecentReadingsRule : IMetricUpdateRule
{
    private const int MaxReadings = 6; // Assuming 10-second intervals for 1 minute

    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        var newQueue = new ConcurrentQueue<Metric>(aggregate.RecentReadings);
        newQueue.Enqueue(newMetric);
        while (newQueue.Count > MaxReadings)
        {
            newQueue.TryDequeue(out _);
        }

        var newAggregate = aggregate.CopyWith(
            recentReadings: newQueue
        );
        return newAggregate;
    }
}
