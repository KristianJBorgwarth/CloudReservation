using AutoMapper;
using CloudReservation.Service.Models.NotificationModels;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Models.Profiles;

public class NotificationEventProfile : Profile
{
    public NotificationEventProfile()
    {
        CreateMap<Event, NotificationEventDto>()
            .ForMember(dest=> dest.ICalUId, opt => opt.MapFrom(src => src.ICalUId))
            .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Location!.DisplayName))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => ConvertToDateTime(src.Start!)))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => ConvertToDateTime(src.End!)));
    }

    private DateTime ConvertToDateTime(DateTimeTimeZone dateTimeTimeZone)
    {
        if (DateTime.TryParse(dateTimeTimeZone.DateTime, out DateTime dateTime))
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(dateTimeTimeZone.TimeZone!);
            return TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo);
        }
        else
        {
            throw new InvalidOperationException("Invalid date format");
        }
    }
}