using Application.Models;

namespace Application.Rules;

public class MinRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, double newValue)
    {
        var newAggregate = aggregate.CopyWith(
            minValue: Math.Min(aggregate.MinValue, newValue)
        );
        return newAggregate;
    }
}
