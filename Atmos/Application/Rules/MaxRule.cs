using Application.Models;

namespace Application.Rules;

public class MaxRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        if (aggregate.MaxValue.Timestamp.Day != newMetric.Timestamp.Day)
        {
            // Reset the daily maximum if the day has changed
            return aggregate.CopyWith(
                maxValue: newMetric
            );
        }

        var abs = Math.Abs(Math.Max(aggregate.MaxValue.Value, newMetric.Value) - aggregate.MaxValue.Value);
        var newAggregate = aggregate.CopyWith(
            maxValue: abs < float.Epsilon ?
                aggregate.MaxValue :
                newMetric
        );
        return newAggregate;
    }
}
