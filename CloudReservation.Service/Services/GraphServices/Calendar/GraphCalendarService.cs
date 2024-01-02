using CloudReservation.Service.Services.GraphServices.Builder;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Services.GraphServices.Calendar;

public class GraphCalendarService : IGraphCalendarService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly string _roomListIdentifier;

    public GraphCalendarService(IGraphBuilder graphBuilder, IConfigurationWrapper configurationWrapper)
    {
        _roomListIdentifier = configurationWrapper.GetMsGraphConfiguration<string>(ConfigurationType.RoomListEmail);
        _graphServiceClient = graphBuilder.Build();
    }

    public async Task<List<Room>> GetListOfRooms()
    {
        var roomList = await _graphServiceClient.Places[_roomListIdentifier].GraphRoomList.GetAsync(
            (requestConfiguration) => { requestConfiguration.QueryParameters.Expand = new string[] { "rooms" }; });

        return roomList?.Rooms ?? new List<Room>();
    }

    public async Task<EventCollectionResponse> GetEventsByDateTime(string roomEmail, DateTimeOffset start, DateTimeOffset end)
    {
        var result = await _graphServiceClient.Users[roomEmail].CalendarView.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.StartDateTime = start.ToString("O");
            requestConfiguration.QueryParameters.EndDateTime = end.ToString("O");
        });

        return result ?? new EventCollectionResponse();
    }

    public async Task<Event> GetEventById(string roomEmail, string eventId)
    {
        var result = await _graphServiceClient.Users[roomEmail].Calendar.Events[eventId].GetAsync();

        return result ?? new Event();
    }

    public async Task<Event?> CreateCalendarEvent(string roomEmail, Event newEvent)
    {
        var response = await _graphServiceClient.Users[roomEmail].Calendar.Events.PostAsync(newEvent);

        return response;
    }

    public async Task DeleteCalendarEvent(string roomEmail, string iCalUId)
    {
        var eventId = await GetEventIdFromICalUId(roomEmail, iCalUId);

        foreach (var id in eventId)
        {
            await _graphServiceClient.Users[roomEmail].Calendar.Events[id].DeleteAsync();
        }
    }

    private async Task<List<string>> GetEventIdFromICalUId(string roomEmail, string iCalUId)
    {
        var result = await _graphServiceClient.Users[roomEmail].Calendar.Events.GetAsync((requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Filter = $"iCalUId eq '{iCalUId}'";
        }));

        var ids = result.Value.Select(item => item.Id).ToList();

        return ids;
    }
}