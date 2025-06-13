using Application.Models;

namespace Application.Rules;

public class MinRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        if (aggregate.MinValue.Timestamp.Day != newMetric.Timestamp.Day)
        {
            // Reset the daily minimum if the day has changed
            return aggregate.CopyWith(
                minValue: newMetric
            );
        }

        var abs = Math.Abs(Math.Min(aggregate.MinValue.Value, newMetric.Value) - aggregate.MinValue.Value);
        var newAggregate = aggregate.CopyWith(
            minValue: abs < float.Epsilon ?
                aggregate.MinValue :
                newMetric
        );
        return newAggregate;
    }
}
