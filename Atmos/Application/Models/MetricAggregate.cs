using System.Collections.Concurrent;

namespace Application.Models;

public class MetricAggregate
{
    /// <summary>
    ///  Current value of the metric. This is the latest reading or the last calculated value.
    /// </summary>
    public Metric CurrentValue { get; init; } = new();

    /// <summary>
    ///  The last six, ten-second readings. Used for calculating the one-minute rolling average.
    /// </summary>
    public ConcurrentQueue<Metric> RecentReadings { get; init; } = [];

    /// <summary>
    /// Daily minimum value. Resets at a configurable time each day (default is midnight UTC).
    /// </summary>
    public Metric MinValue { get; init; } = new() { Value = double.MaxValue };

    /// <summary>
    ///  Daily maximum value. Resets at a configurable time each day (default is midnight UTC).
    /// </summary>
    public Metric MaxValue { get; init; } = new() { Value = double.MinValue };

    /// <summary>
    /// Average of the last six ten-second readings, taken at :10, :20, :30, :40, :50, and :00 each minute.
    /// The one-minute average is updated at the top of the minute (:00), which also triggers an update of the five-minute moving average.
    /// Returns null until six readings are available.
    /// </summary>
    public double? OneMinuteAverage { get; set; }

    /// <summary>
    /// The last five, one-minute averages. Used for calculating the five-minute rolling average.
    /// </summary>
    public ConcurrentQueue<Metric> OneMinuteAverages { get; init; } = [];

    /// <summary>
    /// Average of the last five, one-minute readings. Null until five readings are available.
    /// </summary>
    public double? FiveMinuteRollingAverage => OneMinuteAverages is { IsEmpty: false, Count: 5 }
        ? OneMinuteAverages.Select(m => m.Value).Average()
        : null;

    public MetricAggregate CopyWith(
        Metric? currentValue = null,
        Metric? minValue = null,
        Metric? maxValue = null,
        double? oneMinuteAverage = null,
        ConcurrentQueue<Metric>? recentReadings = null,
        ConcurrentQueue<Metric>? oneMinuteRollingAverages = null)
    {
        return new MetricAggregate
        {
            CurrentValue = currentValue ?? CurrentValue,
            MinValue = minValue ?? MinValue,
            MaxValue = maxValue ?? MaxValue,
            OneMinuteAverage = oneMinuteAverage ?? OneMinuteAverage,
            RecentReadings = recentReadings ?? RecentReadings,
            OneMinuteAverages = oneMinuteRollingAverages ?? OneMinuteAverages
        };
    }

    public override string ToString()
    {
        return $"Current: {CurrentValue}, Min: {MinValue}, Max: {MaxValue}, " +
               $"1-Min Avg: {OneMinuteAverage?.ToString("F2") ?? "N/A"}, " +
               $"5-Min Avg: {FiveMinuteRollingAverage?.ToString("F2") ?? "N/A"} ";
    }
}
