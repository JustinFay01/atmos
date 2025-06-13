using Application.DTOs;


namespace Application.Models;

public class DashboardUpdate
{
    public required ReadingDto LatestReading { get; set; }
    public required MetricAggregate Temperature { get; set; }
    public required MetricAggregate Humidity { get; set; }
    public required MetricAggregate DewPoint { get; set; }
}
