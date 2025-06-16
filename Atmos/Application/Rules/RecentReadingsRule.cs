using System.Collections.Concurrent;

using Application.Dtos;
using Application.Models;

namespace Application.Rules;

/// <summary>
/// Updates the recent readings queue with the latest value. Making sure it only keeps the last six readings.
/// </summary>
public class RecentReadingsRule : IMetricUpdateRule
{
    private const int MaxReadings = 6; // Assuming 10-second intervals for 1 minute

    public SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto)
    {
        var newQueue = new ConcurrentQueue<MetricDto>(aggregateDto.RecentReadings);
        newQueue.Enqueue(newMetricDto);
        while (newQueue.Count > MaxReadings)
        {
            newQueue.TryDequeue(out _);
        }

        var newAggregate = aggregateDto.CopyWith(
            recentReadings: newQueue
        );
        return newAggregate;
    }
}
