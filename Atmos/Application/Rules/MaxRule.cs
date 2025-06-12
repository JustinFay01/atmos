using Application.Models;

namespace Application.Rules;

public class MaxRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        var abs = Math.Abs(Math.Max(aggregate.MaxValue.Value, newMetric.Value) - aggregate.MaxValue.Value);
        var newAggregate = aggregate.CopyWith(
            maxValue: abs < float.Epsilon ?
                aggregate.MaxValue :
                newMetric
        );
        return newAggregate;
    }
}
