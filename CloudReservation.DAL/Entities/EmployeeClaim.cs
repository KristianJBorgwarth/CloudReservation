using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudReservation.DAL.Entities;

public class EmployeeClaim : BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
    public Employee Employee { get; set; } = null!;
}