namespace CloudReservation.DAL.Entities;

public class BaseEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime Updated { get; set; }
}