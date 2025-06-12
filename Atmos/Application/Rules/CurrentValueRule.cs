using Application.Models;

namespace Application.Rules;

public class CurrentValueRule : IMetricUpdateRule
{
    public MetricAggregate Apply(MetricAggregate aggregate, double newSensorValue)
    {
        var newAggregate = aggregate.CopyWith(
            currentValue: newSensorValue
        );
        return newAggregate;
    }
}
