using CloudReservation.DAL.Entities;

namespace CloudReservation.DAL.Repositories;

public interface IUserRepository
{
    public Task AddUserAsync(Employee employee, CancellationToken cancellationToken);
    public Employee? GetUserByName(string name);
    public Task<Employee?> GetUserByNameAsync(string name, CancellationToken cancellationToken);
    public Task<Employee?> GetUserByIdPinCode(string pinCode, CancellationToken cancellationToken);
    public Task<Employee?> GetUserByNameIncludeClaimsAsync(string name, CancellationToken cancellationToken);
    public Task<Employee[]> GetUsersAsync(CancellationToken cancellationToken);
    public Task DeleteUserAsync(Employee employee, CancellationToken cancellationToken);
    public Task AddClaimsToUserAsync(Employee employee, IEnumerable<EmployeeClaim> claims, CancellationToken cancellationToken);
}