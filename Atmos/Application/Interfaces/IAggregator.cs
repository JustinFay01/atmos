using Application.DTOs;
using Application.Models;

namespace Application.Interfaces;

public interface IAggregator
{
    public MetricAggregate Temperature { get; }
    public MetricAggregate Humidity { get; }
    public MetricAggregate DewPoint { get; }
    public Task ProcessReadingAsync(ReadingDto reading, CancellationToken cancellationToken = default);
}
