using Domain.Entities;

namespace Application.Models;

public class DashboardUpdate
{
    public Reading LatestReading { get; set; }
    public MetricAggregate Temperature { get; set; }
    public MetricAggregate Humidity { get; set; }
    public MetricAggregate DewPoint { get; set; }
    public List<Reading> RecentHistory { get; set; }
}
