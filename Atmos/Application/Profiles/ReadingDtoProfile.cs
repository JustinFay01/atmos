using Application.DTOs;

using AutoMapper;

using Domain.Entities;

namespace Application.Profiles;

public class ReadingDtoProfile : Profile
{
    public ReadingDtoProfile()
    {
        CreateMap<ReadingDto, Reading>()
            // Both will be handled by the database on insert/update
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TimeStamp, opt => opt.Ignore());
    }
}
