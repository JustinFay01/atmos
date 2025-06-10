namespace Domain.Entities;

public class Reading : BaseEntity
{
    /// <summary>
    /// Server-side timestamp of when the reading was taken. Indexed for performance. Stored in UTC.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    public double Temperature { get; set; }

    public double DewPoint { get; set; }

    public double Humidity { get; set; }
}
