using Application.Models;

namespace Application.Rules;

public class CurrentValueRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        var newAggregate = aggregate.CopyWith(
            currentValue: newMetric
        );
        return newAggregate;
    }
}
