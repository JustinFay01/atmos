using Application.Dtos;
using Application.Models;

namespace Application.Rules;

public class MinRule : IMetricUpdateRule
{
    public SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto)
    {
        if (aggregateDto.MinValue.Timestamp.Day != newMetricDto.Timestamp.Day)
        {
            // Reset the daily minimum if the day has changed
            return aggregateDto.CopyWith(
                minValue: newMetricDto
            );
        }

        var abs = Math.Abs(Math.Min(aggregateDto.MinValue.Value, newMetricDto.Value) - aggregateDto.MinValue.Value);
        var newAggregate = aggregateDto.CopyWith(
            minValue: abs < float.Epsilon ?
                aggregateDto.MinValue :
                newMetricDto
        );
        return newAggregate;
    }
}
