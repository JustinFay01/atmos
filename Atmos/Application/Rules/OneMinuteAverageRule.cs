using System.Collections.Concurrent;

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
    private const int MaxReadings = 6; // Assuming six ten-second readings for one minute
    private const int ToleranceInSeconds = 4; // Allowable difference in seconds for the one-minute average

    private readonly ILogger<OneMinuteAverageRule> _logger;

    public OneMinuteAverageRule(ILogger<OneMinuteAverageRule> logger)
    {
        _logger = logger;
    }

    public MetricAggregate Apply(MetricAggregate aggregate, Metric newMetric)
    {
        if (aggregate.RecentReadings.IsEmpty || aggregate.RecentReadings.Count < MaxReadings)
        {
            return aggregate;
        }

        // Needs to start at the top of the minute (:10)
        // This corresponds to newMetric's time because the previous rule ensures that the last reading is the latest one.
        var latestRecentReading = aggregate.RecentReadings.Last();
        var isTopOfTheMinute = Math.Abs(latestRecentReading.Timestamp.Second) <= ToleranceInSeconds ||
                                latestRecentReading.Timestamp.Second > 59 - ToleranceInSeconds;

        if (!isTopOfTheMinute)
        {
            return aggregate;
        }

        var firstReading = aggregate.RecentReadings.First();
        var expectedFirstReadingTimestamp = latestRecentReading.Timestamp.AddSeconds(-50);

        if (Math.Abs((firstReading.Timestamp - expectedFirstReadingTimestamp).TotalSeconds) > ToleranceInSeconds)
        {
            _logger.LogError("Clock drift detected, first reading is not at the top of the minute.");
            return aggregate;
        }

        var oneMinuteAverage = aggregate.RecentReadings.Average(m => m.Value);
        var newOneMinuteAverageQueue = new ConcurrentQueue<Metric>(aggregate.OneMinuteAverages);
        newOneMinuteAverageQueue.Enqueue(new Metric
        {
            Timestamp = latestRecentReading.Timestamp,
            Value = oneMinuteAverage
        });

        while (newOneMinuteAverageQueue.Count > MaxReadings)
        {
            newOneMinuteAverageQueue.TryDequeue(out _);
        }

        var newAggregate = aggregate.CopyWith(
            oneMinuteAverage: oneMinuteAverage,
            oneMinuteRollingAverages: newOneMinuteAverageQueue
        );
        return newAggregate;
    }
}
