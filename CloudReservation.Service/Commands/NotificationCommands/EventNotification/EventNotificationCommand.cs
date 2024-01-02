using AutoMapper;
using CloudReservation.Service.Hubs;
using CloudReservation.Service.Models.NotificationModels;
using CloudReservation.Service.Services.GraphServices.Calendar;
using CloudReservation.Service.Services.NotificationCacheService;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Commands.NotificationCommands.EventNotification;

public record EventNotificationCommand() : IRequest<Result<Unit>>
{
    public required NotificationWrapper NotificationWrapper { get; init; }
};

public class EventNotificationCommandHandler : IRequestHandler<EventNotificationCommand, Result<Unit>>
{
    private readonly INotificationCacheService _notificationCacheService;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IGraphCalendarService _calendarService;

    public EventNotificationCommandHandler(INotificationCacheService notificationCacheService, IMapper mapper, IHubContext<NotificationHub> hubContext, IGraphCalendarService calendarService)
    {
        _notificationCacheService = notificationCacheService;
        _mapper = mapper;
        _hubContext = hubContext;
        _calendarService = calendarService;
    }


    public async Task<Result<Unit>> Handle(EventNotificationCommand request, CancellationToken cancellationToken)
    {
        foreach (var notification in request.NotificationWrapper.Value)
        {
            if (_notificationCacheService.IsDuplicateNotification(notification))
            {
                return Result.Fail("Duplicate notification received. Skipping.");
            }

            var roomId = GetIdFromResource(notification.Resource);

            var meeting = await _calendarService.GetEventById(roomId, notification.ResourceData.Id);

            var eventDto = _mapper.Map<Event, NotificationEventDto>(meeting);

            await _hubContext.Clients.Group(eventDto.Room).SendAsync("ReceiveNotification", eventDto, cancellationToken: cancellationToken);

            Console.WriteLine(eventDto.ICalUId);
            Console.WriteLine(eventDto.Subject);
            Console.WriteLine(eventDto.Room);
            Console.WriteLine(eventDto.Start);
            Console.WriteLine(eventDto.End);

        }

        return Result.Ok(Unit.Value);
    }

    private static string GetIdFromResource(string resource)
    {
        var resourceArray = resource.Split('/');

        return resourceArray[1];
    }
}