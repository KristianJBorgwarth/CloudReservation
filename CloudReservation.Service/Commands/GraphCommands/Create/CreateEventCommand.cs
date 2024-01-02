using AutoMapper;
using CloudReservation.Service.Models.CalendarModels;
using CloudReservation.Service.Services.GraphServices.Calendar;
using FluentResults;
using MediatR;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Commands.GraphCommands.Create
{
    public record CreateEventCommand() : IRequest<Result<CalendarEventDto>>
    {
        public string? Subject { get; init; }
        public required DateTime Start { get; init; }
        public required DateTime End { get; init; }
        public string? Organizer { get; set; }
        public required string RoomEmail { get; init; }
    };

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<CalendarEventDto>>
    {
        private readonly IMapper _mapper;
        private readonly IGraphCalendarService _calendarService;
        public CreateEventCommandHandler(IMapper mapper, IGraphCalendarService calendarService)
        {
            _mapper = mapper;
            _calendarService = calendarService;
        }

        public async Task<Result<CalendarEventDto>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = _mapper.Map<Event>(request);

            newEvent.IsOrganizer = true;
            newEvent.ResponseRequested = true;

            var result = await _calendarService.CreateCalendarEvent(request.RoomEmail, newEvent);

            var calendarEvent = _mapper.Map<CalendarEventDto>(result);

            return Result.Ok(calendarEvent);
        }
    }
}
