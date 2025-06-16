using Application.Dtos;
using Application.Models;

namespace Application.Rules;

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
