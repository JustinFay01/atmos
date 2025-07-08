namespace Domain.Entities;

public class ReadingAggregate : BaseEntity
{
    /// <summary>
    /// The time that this aggregate represents.
    /// Stored in UTC.
    /// </summary>
    public DateTime Timestamp { get; set; }

    #region Temperature

    /// <summary>
    /// The temperature recorded at the Timestamp's point in time.
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// The time that the minimum temperature occurred at the Timestamp's point in time.
    /// Stored in UTC.
    /// </summary>
    public DateTime TemperatureMinTime { get; set; }
    /// <summary>
    /// The day's minimum temperature recorded at the Timestamp's point in time.
    /// </summary>
    public double TemperatureMin { get; set; }

    /// <summary>
    /// The time that the maximum temperature occurred at the Timestamp's point in time.
    /// Stored in UTC.
    /// </summary>
    public DateTime TemperatureMaxTime { get; set; }

    /// <summary>
    /// The day's maximum temperature recorded at the Timestamp's point in time.
    /// Stored in UTC.
    ///  </summary>
    public double TemperatureMax { get; set; }

    /// <summary>
    /// The average calculated from the last 60 seconds of 10-second temperature readings.
    /// If the program has been running for less than 60 seconds, this will be null.
    /// </summary>
    public double? TemperatureOneMinuteAverage { get; set; }

    /// <summary>
    /// The rolling average calculated from the last 5 minutes of one-minute temperature averages.
    /// This will be updated every minute after the first 5 one-minute averages have been calculated.
    /// </summary>
    public double? TemperatureFiveMinuteRollingAverage { get; set; }

    #endregion

    #region Humidity

    public double Humidity { get; set; }
    public DateTime HumidityMinTime { get; set; }
    public double HumidityMin { get; set; }
    public DateTime HumidityMaxTime { get; set; }
    public double HumidityMax { get; set; }
    public double? HumidityOneMinuteAverage { get; set; }
    public double? HumidityFiveMinuteRollingAverage { get; set; }
    #endregion

    #region DewPoint

    public double DewPoint { get; set; }
    public DateTime DewPointMinTime { get; set; }
    public double DewPointMin { get; set; }
    public DateTime DewPointMaxTime { get; set; }
    public double DewPointMax { get; set; }
    public double? DewPointOneMinuteAverage { get; set; }
    public double? DewPointFiveMinuteRollingAverage { get; set; }

    #endregion
}
