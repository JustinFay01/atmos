using Application.Dtos;
using Application.Models;

namespace Application.Rules;

public class MaxRule : IMetricUpdateRule
{
    public SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto)
    {
        if (aggregateDto.MaxValue.Timestamp.Day != newMetricDto.Timestamp.Day)
        {
            // Reset the daily maximum if the day has changed
            return aggregateDto.CopyWith(
                maxValue: newMetricDto
            );
        }

        var abs = Math.Abs(Math.Max(aggregateDto.MaxValue.Value, newMetricDto.Value) - aggregateDto.MaxValue.Value);
        var newAggregate = aggregateDto.CopyWith(
            maxValue: abs < float.Epsilon ?
                aggregateDto.MaxValue :
                newMetricDto
        );
        return newAggregate;
    }
}
