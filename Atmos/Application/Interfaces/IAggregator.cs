using Atmos.Application.Dtos;
using Atmos.Application.Models;

namespace Atmos.Application.Interfaces;

public interface IAggregator
{
    public ReadingAggregateDto? AggregatedReading { get; }
    public Task<ReadingAggregateDto> AggregateRawReading(RawSensorReading reading, CancellationToken cancellationToken = default);
}
