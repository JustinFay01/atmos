using Application.Models;

using Domain.Entities;

namespace Application.Interfaces;

public interface IAggregator
{
    public MetricAggregate Temperature { get; }
    public MetricAggregate Humidity { get; }
    public MetricAggregate DewPoint { get; }
    public Task ProcessReadingAsync(Reading reading, CancellationToken cancellationToken = default);
}
