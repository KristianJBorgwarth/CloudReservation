using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.Service.Models.RoomModels;
using CloudReservation.Service.Services.GraphServices.Calendar;
using FluentResults;
using MediatR;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Queries.RoomQueries.GetRooms
{
    public class GetRoomListQuery : IRequest<Result<IEnumerable<RoomDto>>> { }

    public class GetRoomListQueryHandler : IRequestHandler<GetRoomListQuery, Result<IEnumerable<RoomDto>>>
    {
        private readonly IGraphCalendarService _graphCalendarService;
        private readonly IMapper _mapper;

        public GetRoomListQueryHandler(CloudReservationDbContext db, IMapper mapper, IGraphCalendarService graphCalendarService)
        {
            _mapper = mapper;
            _graphCalendarService = graphCalendarService;
        }

        public async Task<Result<IEnumerable<RoomDto>>> Handle(GetRoomListQuery request, CancellationToken cancellationToken)
        {
            var rooms = await _graphCalendarService.GetListOfRooms();

            if (!rooms.Any())
                return Result.Fail("No rooms found");

            var mappedRooms = rooms.Select(roomDto => _mapper.Map<Room, RoomDto>(roomDto)).ToList();

            return mappedRooms;
        }
    }
}
