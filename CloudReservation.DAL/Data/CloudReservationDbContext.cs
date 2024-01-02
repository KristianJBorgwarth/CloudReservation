using CloudReservation.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudReservation.DAL.Data;

public class CloudReservationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<EmployeeClaim> EmployeeClaims { get; set; } = null!;

    public CloudReservationDbContext(DbContextOptions<CloudReservationDbContext> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            //TODO: substitute hardcoded user with user from header
            var currentUser = "John Doe";
            var timestamp = DateTime.Now;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = timestamp;
                    entry.Entity.CreatedBy = currentUser;
                    break;
                case EntityState.Modified:
                    entry.Entity.Updated = timestamp;
                    entry.Entity.UpdatedBy = currentUser;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}