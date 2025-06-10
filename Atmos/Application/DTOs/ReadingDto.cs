namespace Application.DTOs;

public class ReadingDto
{
    public required double Temperature { get; set; }
    public required double DewPoint { get; set; }
    public required double Humidity { get; set; }
}