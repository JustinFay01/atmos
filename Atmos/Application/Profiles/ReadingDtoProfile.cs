using Application.DTOs;

using AutoMapper;

using Domain.Entities;

namespace Application.Profiles;

public class ReadingDtoProfile : Profile
{
    public ReadingDtoProfile()
    {
        CreateMap<ReadingDto, Reading>();
    }
}
