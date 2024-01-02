// ReSharper disable InconsistentNaming
namespace CloudReservation.Service.Models.NotificationModels;

public class NotificationEventDto
{
    public required string ICalUId { get; init; }
    public required string Room { get; init; }
    public  string? Subject { get; set; }
    public required DateTime Start { get; init; }
    public required DateTime End { get; init; }
}