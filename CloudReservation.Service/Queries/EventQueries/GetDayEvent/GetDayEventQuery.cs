using AutoMapper;
using CloudReservation.Service.Models.CalendarModels;
using CloudReservation.Service.Services.GraphServices.Calendar;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Queries.EventQueries.GetDayEvent
{
    public class GetDayEventQuery : IRequest<Result<IEnumerable<CalendarEventDto>>>
    {
        [FromQuery]
        public required string RoomEmail { get; init; }
        public required DateTimeOffset Start { get; init; }
        public required DateTimeOffset End { get; init; }
    }

    public class GetDayEventQueryHandler : IRequestHandler<GetDayEventQuery, Result<IEnumerable<CalendarEventDto>>>
    {
        private readonly IGraphCalendarService _calendarService;
        private readonly IMapper _mapper;
        public GetDayEventQueryHandler(IGraphCalendarService calendarService, IMapper mapper)
        {
            _calendarService = calendarService;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CalendarEventDto>>> Handle(GetDayEventQuery request, CancellationToken cancellationToken)
        {
            var result = await _calendarService.GetEventsByDateTime(request.RoomEmail, request.Start, request.End);

            var events = _mapper.Map<IEnumerable<CalendarEventDto>>(result.Value);

            return IsResultValid(result) ? Result.Ok(events) : Result.Fail<IEnumerable<CalendarEventDto>>("No events found");
        }

        private static bool IsResultValid(EventCollectionResponse events)
        {
            return events.Value is not null || events.Value?.Count > 0;
        }

    }
}
