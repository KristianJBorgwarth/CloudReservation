using CloudReservation.Service.Services.GraphServices.Calendar;
using FluentResults;
using MediatR;
// ReSharper disable InconsistentNaming

namespace CloudReservation.Service.Commands.GraphCommands.Delete
{
    public record DeleteEventCommand() : IRequest<Result<Unit>>
    {
        public required string ICalUId { get; init; }
        public required string RoomEmail { get; init; }
    };
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<Unit>>
    {
        private readonly IGraphCalendarService _calendarService;

        public DeleteEventCommandHandler(IGraphCalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<Result<Unit>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            await _calendarService.DeleteCalendarEvent(request.RoomEmail, request.ICalUId);

            return new Result<Unit>();
        }
    }
}
