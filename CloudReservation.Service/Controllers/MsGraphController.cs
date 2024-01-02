using CloudReservation.Service.Queries.RoomQueries;
using CloudReservation.Service.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CloudReservation.Service.Queries.EventQueries;
using CloudReservation.Service.Commands.GraphCommands;
using CloudReservation.Service.Commands.GraphCommands.Create;
using CloudReservation.Service.Commands.GraphCommands.Delete;
using CloudReservation.Service.Models.CalendarModels;
using CloudReservation.Service.Models.RoomModels;
using CloudReservation.Service.Queries.EventQueries.GetDayEvent;
using CloudReservation.Service.Queries.RoomQueries.GetRooms;
using FluentResults;
using Microsoft.AspNetCore.Authorization;

namespace CloudReservation.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MsGraphController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MsGraphController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a a list of bookable rooms in outlook
        /// </summary>
        /// <returns>Returns an IEnumerable of RoomsDto</returns>
        [HttpGet("rooms")]
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> GetRooms()
        {
            var result = await _mediator.Send(new GetRoomListQuery());

            return result.IsSuccess ? Results.Ok(result.Value) : Results.NoContent();
        }

        /// <summary>
        /// Retrieves a list of events for a specified room and date
        /// </summary>
        /// <param name="query">Query specifying the parameters for search</param>
        /// <returns>An IEnumerable of CalendarEventDto</returns>
        [HttpGet("events")]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> GetDayEvents([FromQuery] GetDayEventQuery query)
        {
            var result = await _mediator.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.NoContent();
        }

        /// <summary>
        /// Creates a new event in the specified room calendar
        /// </summary>
        /// <param name="cmd">Command containing the event for creation</param>
        /// <returns>The created event</returns>
        [HttpPost("create-event")]
        [Authorize(Policy = "MeetingRoomAccess")]
        [ProducesResponseType(typeof(CalendarEventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IResult> CreateEvent([FromBody] CreateEventCommand cmd)
        {
            var email = User.GetEmail();
            cmd.Organizer = email;

            var result = await _mediator.Send(cmd);

            return result.IsSuccess ? Results.Created(nameof(CreateEvent), result.Value) : result.Errors.ConvertErrorsToBadRequestResult(StatusCodes.Status401Unauthorized);
        }

        [HttpPost("delete-event")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> DeleteEvent([FromBody] DeleteEventCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            return result.IsSuccess ? Results.Ok(result.Value) : result.Errors.ConvertErrorsToBadRequestResult(StatusCodes.Status401Unauthorized);
        }
    }
}
