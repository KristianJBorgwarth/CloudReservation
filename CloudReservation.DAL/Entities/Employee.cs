using System.ComponentModel.DataAnnotations;

namespace CloudReservation.DAL.Entities;

public class Employee : BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PinCode { get; set; }
    public ICollection<EmployeeClaim> Claims { get; set; } = new List<EmployeeClaim>();
}