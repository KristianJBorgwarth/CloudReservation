using Microsoft.Graph.Models;

namespace CloudReservation.Service.Services.GraphServices.Calendar;

public interface IGraphCalendarService
{
    public Task<List<Room>> GetListOfRooms();
    public Task<EventCollectionResponse> GetEventsByDateTime(string roomEmail, DateTimeOffset start, DateTimeOffset end);
    public Task<Event?> CreateCalendarEvent(string roomEmail, Event newEvent);
    public Task DeleteCalendarEvent(string roomEmail, string iCalUId);
    public Task<Event> GetEventById(string roomId, string resourceDataId);
}