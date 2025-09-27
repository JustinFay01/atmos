using Atmos.Application.Models;

using AutoMapper;

using Domain.Entities;

namespace Atmos.Application.Profiles;

public class RawSensorReadingToAggregateProfile : Profile
{
    public RawSensorReadingToAggregateProfile()
    {
        CreateMap<RawSensorReading, ReadingAggregate>()
            // Both will be handled by the database on insert/update
            .ForMember(dest => dest.Timestamp, opt => opt.Ignore());
    }
}
