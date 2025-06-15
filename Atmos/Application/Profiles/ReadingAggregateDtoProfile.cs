using Application.Dtos;

using AutoMapper;

using Domain.Entities;

namespace Application.Profiles;

public class ReadingAggregateDtoProfile : Profile
{
    public ReadingAggregateDtoProfile()
    {
        // Flatten the ReadingAggregateDto to a single object for ease of storage in the database.
        CreateMap<ReadingAggregateDto, ReadingAggregate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            // Raw Reading Values
            .ForMember(dest => dest.Timestamp,
                opt => opt.MapFrom(src => src.LatestReading.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.LatestReading.Temperature))
            .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.LatestReading.Humidity))
            .ForMember(dest => dest.DewPoint, opt => opt.MapFrom(src => src.LatestReading.DewPoint))
            // Min/Max 
            .ForMember(dest => dest.TemperatureMinTime,
                opt => opt.MapFrom(src => src.Temperature.MinValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.TemperatureMin, opt => opt.MapFrom(src => src.Temperature.MinValue.Value))
            .ForMember(dest => dest.TemperatureMaxTime,
                opt => opt.MapFrom(src => src.Temperature.MaxValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.TemperatureMax, opt => opt.MapFrom(src => src.Temperature.MaxValue.Value))
            .ForMember(dest => dest.HumidityMinTime,
                opt => opt.MapFrom(src => src.Humidity.MinValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.HumidityMin, opt => opt.MapFrom(src => src.Humidity.MinValue.Value))
            .ForMember(dest => dest.HumidityMaxTime,
                opt => opt.MapFrom(src => src.Humidity.MaxValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.HumidityMax, opt => opt.MapFrom(src => src.Humidity.MaxValue.Value))
            .ForMember(dest => dest.DewPointMinTime,
                opt => opt.MapFrom(src => src.DewPoint.MinValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.DewPointMin, opt => opt.MapFrom(src => src.DewPoint.MinValue.Value))
            .ForMember(dest => dest.DewPointMaxTime,
                opt => opt.MapFrom(src => src.DewPoint.MaxValue.Timestamp.ToUniversalTime()))
            .ForMember(dest => dest.DewPointMax, opt => opt.MapFrom(src => src.DewPoint.MaxValue.Value))
            // One-minute average
            .ForMember(dest => dest.TemperatureOneMinuteAverage,
                opt => opt.MapFrom(src => src.Temperature.OneMinuteAverage))
            .ForMember(dest => dest.HumidityOneMinuteAverage,
                opt => opt.MapFrom(src => src.Humidity.OneMinuteAverage))
            .ForMember(dest => dest.DewPointOneMinuteAverage,
                opt => opt.MapFrom(src => src.DewPoint.OneMinuteAverage))
            // Five-minute rolling average
            .ForMember(dest => dest.TemperatureFiveMinuteRollingAverage,
                opt => opt.MapFrom(src => src.Temperature.FiveMinuteRollingAverage))
            .ForMember(dest => dest.HumidityFiveMinuteRollingAverage,
                opt => opt.MapFrom(src => src.Humidity.FiveMinuteRollingAverage))
            .ForMember(dest => dest.DewPointFiveMinuteRollingAverage,
                opt => opt.MapFrom(src => src.DewPoint.FiveMinuteRollingAverage));
    }
}
