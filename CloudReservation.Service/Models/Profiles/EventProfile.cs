using AutoMapper;
using CloudReservation.Service.Commands.GraphCommands;
using CloudReservation.Service.Commands.GraphCommands.Create;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Models.Profiles;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<CreateEventCommand, Event>()
            .ForMember(dest => dest.Organizer, opt => opt.Ignore())
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => new DateTimeTimeZone
            {
                DateTime = src.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = "Europe/Berlin"
            }))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => new DateTimeTimeZone
            {
                DateTime = src.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = "Europe/Berlin"
            }))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new Location { DisplayName = src.RoomEmail }))
            .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => new List<Attendee>
            {
                new Attendee
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = src.RoomEmail
                    },
                    Type = AttendeeType.Required,
                    Status = new ResponseStatus()
                    {
                        Response = ResponseType.TentativelyAccepted,
                        Time = DateTimeOffset.Now
                    }
                },
                new Attendee()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = src.Organizer
                    },
                    Type = AttendeeType.Required,
                    Status = new ResponseStatus()
                    {
                        Response = ResponseType.TentativelyAccepted,
                        Time = DateTimeOffset.Now
                    },

                }
            }));
    }
}