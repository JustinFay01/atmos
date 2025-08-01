using System.Collections.Concurrent;

using Application.Dtos;
using Application.Models;
using Application.Services;

using Microsoft.Extensions.Logging;

namespace Application.Rules;

/// <summary>
/// Once a minute, at the top of the minute (:00), this rule updates the one-minute rolling average.
/// The FiveMinuteRollingAverage is automatically calculated based on the last five one-minute averages.
/// If there are fewer than five one-minute averages, the FiveMinuteRollingAverage will be null.
///
///
/// To do so, we follow these steps:
///     1. Ensure that the one-minute averages queue has at least five readings.
///     2. Assert that the first reading is at :10 and the last reading is at :00 (+- 4 seconds).
///     3. Calculate the one-minute average from the queue.
///     4. Enqueue the new one-minute average into the one-minute averages queue.
///     5. If the queue exceeds five averages, dequeue the oldest average.
///     6. Create a new `MetricAggregate` with the updated one-minute average and the updated one-minute averages queue.
/// </summary>
public class OneMinuteAverageRule : IMetricUpdateRule
{
    private const int MaxReadings = 5; // We want 5 one-minute averages so that we can calculate a five-minute rolling average.

    private readonly ILogger<OneMinuteAverageRule> _logger;
    private readonly SensorSettings _sensorSettings;

    public OneMinuteAverageRule(ILogger<OneMinuteAverageRule> logger, SensorSettings sensorSettings)
    {
        _logger = logger;
        _sensorSettings = sensorSettings;
    }

    public SingleReadingAggregateDto Apply(SingleReadingAggregateDto aggregateDto, MetricDto newMetricDto)
    {
        if (aggregateDto.RecentReadings.IsEmpty || aggregateDto.RecentReadings.Count < MaxReadings)
        {
            return aggregateDto;
        }

        // Needs to start at the top of the minute (:10)
        // This corresponds to newMetric's time because the previous rule ensures that the last reading is the latest one.
        var latestRecentReading = aggregateDto.RecentReadings.Last();
        var isTopOfTheMinute = Math.Abs(latestRecentReading.Timestamp.Second) <= _sensorSettings.Tolerance / 1000 ||
                                latestRecentReading.Timestamp.Second > 59 - _sensorSettings.Tolerance / 1000;

        if (!isTopOfTheMinute)
        {
            return aggregateDto;
        }

        var firstReading = aggregateDto.RecentReadings.First();
        var expectedFirstReadingTimestamp = latestRecentReading.Timestamp.AddSeconds(-50);

        if (Math.Abs((firstReading.Timestamp - expectedFirstReadingTimestamp).TotalSeconds) > _sensorSettings.Tolerance * 1000)
        {
            _logger.LogWarning("Clock drift detected, first reading is not at the top of the minute.");
        }

        var oneMinuteAverage = aggregateDto.RecentReadings.Average(m => m.Value);
        var newOneMinuteAverageQueue = new ConcurrentQueue<MetricDto>(aggregateDto.OneMinuteAverages);
        newOneMinuteAverageQueue.Enqueue(new MetricDto
        {
            Timestamp = latestRecentReading.Timestamp,
            Value = oneMinuteAverage
        });

        while (newOneMinuteAverageQueue.Count > MaxReadings)
        {
            newOneMinuteAverageQueue.TryDequeue(out _);
        }

        var newAggregate = aggregateDto.CopyWith(
            oneMinuteAverage: oneMinuteAverage,
            oneMinuteRollingAverages: newOneMinuteAverageQueue
        );
        return newAggregate;
    }
}
