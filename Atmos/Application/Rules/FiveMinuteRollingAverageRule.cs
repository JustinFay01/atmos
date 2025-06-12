using System.Collections.Concurrent;

using Application.Models;

namespace Application.Rules;

public class FiveMinuteRollingAverageRule : IMetricUpdateRule
{
    private const int MaxReadings = 5; // Assuming 1-minute intervals for 5 minutes
    
    public MetricAggregate Apply(MetricAggregate aggregate, double newValue)
    {
        var newQueue = new ConcurrentQueue<double>(aggregate.OneMinuteRollingAverages);
        newQueue.Enqueue(newValue);
        while (newQueue.Count > MaxReadings)
        {
            newQueue.TryDequeue(out _);
        }
        
        var newAggregate = aggregate.CopyWith(
            oneMinuteRollingAverages: newQueue
        );
        return newAggregate;    
    }
}
