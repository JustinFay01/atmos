using System.Collections.Concurrent;

namespace Application.Models;

public class MetricAggregate 
{
    /// <summary>
    ///  Current value of the metric. This is the latest reading or the last calculated value.
    /// </summary>
    public double CurrentValue { get; init; } = double.MinValue;

    /// <summary>
    ///  The last six, ten-second readings. Used for calculating the one-minute rolling average.
    /// </summary>
    public ConcurrentQueue<double> RecentReadings { get; init; } = [];

    /// <summary>
    /// Daily minimum value. Resets at a configurable time each day (default is midnight UTC).
    /// </summary>
    public double MinValue { get; init; } = double.MaxValue;

    /// <summary>
    ///  Daily maximum value. Resets at a configurable time each day (default is midnight UTC).
    /// </summary>
    public double MaxValue { get; init; } = double.MinValue;

    /// <summary>
    /// Average of the last six, ten-second readings. Null until six readings are available.
    /// </summary>
    public double? OneMinuteRollingAverage => RecentReadings is { IsEmpty: false, Count: 6 } 
        ? RecentReadings.Average() 
        : null;
    
    /// <summary>
    /// The last five, one-minute rolling averages. Used for calculating the five-minute rolling average.
    /// </summary>
    public ConcurrentQueue<double> OneMinuteRollingAverages { get; init; } = [];

    /// <summary>
    /// Average of the last five, one-minute readings. Null until five readings are available.
    /// </summary>
    public double? FiveMinuteRollingAverage  => OneMinuteRollingAverages is { IsEmpty: false, Count: 5 } 
        ? OneMinuteRollingAverages.Average() 
        : null;
    
    public MetricAggregate CopyWith(
        double? currentValue = null,
        double? minValue = null,
        double? maxValue = null,
        ConcurrentQueue<double>? recentReadings = null,
        ConcurrentQueue<double>? oneMinuteRollingAverages = null)
    {
        return new MetricAggregate
        {
            CurrentValue = currentValue ?? CurrentValue,
            MinValue = minValue ?? MinValue,
            MaxValue = maxValue ?? MaxValue,
            RecentReadings = recentReadings ?? RecentReadings,
            OneMinuteRollingAverages = oneMinuteRollingAverages ?? OneMinuteRollingAverages
        };
    }

    public override string ToString()
    {
        return $"Current: {CurrentValue}, Min: {MinValue}, Max: {MaxValue}, " +
               $"1-Min Avg: {OneMinuteRollingAverage?.ToString("F2") ?? "N/A"}, " +
               $"5-Min Avg: {FiveMinuteRollingAverage?.ToString("F2") ?? "N/A"} ";
    }
}
