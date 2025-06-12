using Application.Models;

namespace Application.Rules;

public interface IMetricUpdateRule
{
    /// <summary>
    /// Applies an update to the metric aggregate based on a new sensor value.
    /// </summary>
    /// <param name="aggregate">The metric aggregate to update.</param>
    /// <param name="newSensorValue">The new sensor value being processed.</param>
    MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric);

}
