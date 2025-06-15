using Application.Models;

using AutoMapper;

using Domain.Entities;

namespace Application.Profiles;

public class RawSensorReadingToAggregateProfile : Profile
{
    public RawSensorReadingToAggregateProfile()
    {
        CreateMap<RawSensorReading, ReadingAggregate>()
            // Both will be handled by the database on insert/update
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Timestamp, opt => opt.Ignore());
    }
}
