using CloudReservation.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.IntegrationUtilities;

[Collection("TestCollection")]
public class IntegrationTestBase : IDisposable
{
    protected IntegrationTestFactory<Program, CloudReservationDbContext> Factory { get; init; }
    protected HttpClient Client { get; init; }
    protected CloudReservationDbContext Db { get; init; }

    public IntegrationTestBase(IntegrationTestFactory<Program, CloudReservationDbContext> factory)
    {
        Factory = factory;

        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<CloudReservationDbContext>();
        
        Client = factory.CreateClient();
        Db.Database.EnsureCreated();
    }

    public virtual void Dispose()
    {
        Db.Employees.ExecuteDelete();
        Db.EmployeeClaims.ExecuteDelete();
    }
}