using Atmos.Application.Models;

using Atmos.Application.Dtos;

namespace Atmos.Application.Rules;

public interface IMetricUpdateRule
{
    /// <summary>
    /// Applies an update to the metric aggregate based on a new sensor value.
    /// </summary>
    /// <param name="aggregateDto">The metric aggregate to update.</param>
    /// <param name="newMetricDto">The new sensor value being processed.</param>
    SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto);

}
