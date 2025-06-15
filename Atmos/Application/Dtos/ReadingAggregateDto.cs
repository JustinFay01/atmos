using Application.Models;

namespace Application.Dtos;

public class ReadingAggregateDto
{
    public required RawSensorReading LatestReading { get; set; }
    public required SingleReadingAggregateDto Temperature { get; set; }
    public required SingleReadingAggregateDto Humidity { get; set; }
    public required SingleReadingAggregateDto DewPoint { get; set; }
}
