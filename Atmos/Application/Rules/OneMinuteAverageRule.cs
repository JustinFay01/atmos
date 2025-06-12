using System.Collections.Concurrent;

using Application.Models;

namespace Application.Rules;

/// <summary>
/// Once a minute, at the top of the minute (:00), this rule updates the one-minute rolling average.
/// The FiveMinuteRollingAverage is automatically calculated based on the last five one-minute averages.
/// If there are fewer than five one-minute averages, the FiveMinuteRollingAverage will be null.
/// </summary>
public class OneMinuteAverageRule : IMetricUpdateRule
{
    private const int MaxReadings = 5; // Assuming 1-minute intervals for 5 minutes

    public MetricAggregate Apply(MetricAggregate aggregate, double newValue)
    {
        // Assert that our first reading beings at :10 and our most recent reading is at :00 (+- 200 ms to account for clock drift)




        return null!;
    }
}
