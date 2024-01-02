using AutoMapper;
using CloudReservation.Service.Models.CalendarModels;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Models.Profiles
{
    public class CalendarEventProfile : Profile
    {
        public CalendarEventProfile()
        {
            CreateMap<Event, CalendarEventDto>()
                .ForMember(dest => dest.iCalUId, opt => opt.MapFrom(src => src.ICalUId))
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Organizer.EmailAddress.Name))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => ConvertToDateTime(src.Start)))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => ConvertToDateTime(src.End)));


        }

        private static DateTime ConvertToDateTime(DateTimeTimeZone dateTimeTimeZone)
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
}