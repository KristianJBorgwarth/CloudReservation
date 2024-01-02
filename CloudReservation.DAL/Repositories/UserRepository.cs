using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudReservation.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CloudReservationDbContext _dbContext;

    public UserRepository(CloudReservationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employee?> GetUserByNameIncludeClaimsAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public Employee? GetUserByName(string name)
    {
        return _dbContext.Employees.FirstOrDefault(c=> c.Name == name);
    }

    //TODO: Should be written tests for this
    public async Task<Employee?> GetUserByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task DeleteUserAsync(Employee employee, CancellationToken cancellationToken)
    {
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddClaimsToUserAsync(Employee employee, IEnumerable<EmployeeClaim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            employee.Claims.Add(claim);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employee?> GetUserByIdPinCode(string pinCode, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.Include(c => c.Claims).FirstOrDefaultAsync(c => c.PinCode == pinCode, cancellationToken);
    }

    public async Task<Employee[]> GetUsersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.Include(c => c.Claims).ToArrayAsync(cancellationToken);
    }
}