
namespace Application.Dtos;

public class HourReadingDto
{
    private readonly int _hour;
    
    /// <summary>
    ///  The hour (0-23) of the reading, representing the hour in a 24-hour format.
    /// </summary>
    public int Hour {
        get => _hour;
        init {
            if (value is < 0 or > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Hour must be between 0 and 23.");
            }
            _hour = value;
        }
    }
    
    public required DateTime Timestamp { get; set; }
    public required double Temperature { get; set; }
    public required double DewPoint { get; set; }
    public required double Humidity { get; set; }
}
