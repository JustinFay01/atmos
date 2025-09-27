using Atmos.Application.Models;

using Atmos.Application.Dtos;

namespace Atmos.Application.Rules;

public class CurrentValueRule : IMetricUpdateRule
{
    public SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto)
    {
        var newAggregate = aggregateDto.CopyWith(
            currentValue: newMetricDto
        );
        return newAggregate;
    }
}
