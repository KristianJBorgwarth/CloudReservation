using Microsoft.AspNetCore.Mvc;

namespace CloudReservation.Service.Queries.EventQueries
{
    public class DeleteEventQuery
    {
        // Event ID
        public string? iCalUId { get; set; }
    }



}
