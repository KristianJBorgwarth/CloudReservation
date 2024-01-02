namespace CloudReservation.Service.Models.CalendarModels
{
    public class CalendarEventDto
    {
        public required string iCalUId { get; init; }
        public string? Organizer { get; set; }
        public string? Subject { get; set; }
        public required DateTime Start { get; init; }
        public required DateTime End { get; init; }
    }
}