using Application.Helper;

namespace Application.DTOs;

public class ReadingDto
{
    public DateTime Timestamp = DateTimeProvider.Instance.Now;
    public required double Temperature { get; set; }
    public required double DewPoint { get; set; }
    public required double Humidity { get; set; }
}
