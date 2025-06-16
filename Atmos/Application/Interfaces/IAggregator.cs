using Application.Dtos;
using Application.Models;

namespace Application.Interfaces;

public interface IAggregator
{
    public ReadingAggregateDto? AggregatedReading { get; }
    public Task<ReadingAggregateDto> AggregateRawReading(RawSensorReading reading, CancellationToken cancellationToken = default);
}
