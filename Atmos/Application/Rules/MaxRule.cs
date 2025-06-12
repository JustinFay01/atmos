using Application.Models;

namespace Application.Rules;

public class MaxRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, double newValue)
    {
        var newAggregate = aggregate.CopyWith(maxValue: Math.Max(aggregate.MaxValue, newValue));
        return newAggregate;
    }
}
