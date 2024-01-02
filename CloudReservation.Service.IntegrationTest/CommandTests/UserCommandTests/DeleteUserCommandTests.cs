using System.Security.Claims;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Delete;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.CommandTests.UserCommandTests;

public class DeleteUserCommandTests : IntegrationTestBase
{
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _handler = new DeleteUserCommandHandler(repo);
    }

    [Fact]
    public async void GivenValidNameInDeleteCommand_UserShouldBeDeleted()
    {
        //Arrange
        var user = new Employee()
        {
            Email = "Test",
            Name = "TestUser",
            PinCode = "Test",
            Claims = new List<EmployeeClaim>()
            {
                new EmployeeClaim()
                {
                    ClaimType = "TestType",
                    ClaimValue = "Test"
                }
            }
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        //Act
        var result = await _handler.Handle(new DeleteUserCommand("TestUser"), CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        Db.Employees.FirstOrDefault(c => c.Name == "TestUser").Should().BeNull();
        Db.EmployeeClaims.FirstOrDefault(c => c.EmployeeId == user.Id).Should().BeNull();
    }

    [Fact]
    public async void GivenInvalidNameInDeleteCommand_ShouldThrowException()
    {
        //Arrange
        var user = new Employee()
        {
            Email = "Test",
            Name = "TestUser",
            PinCode = "Test",
            Claims = new List<EmployeeClaim>()
            {
                new EmployeeClaim()
                {
                    ClaimType = "TestType",
                    ClaimValue = "Test"
                }
            }
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        //Act
        Func<Task> act = async () => await _handler.Handle(new DeleteUserCommand("wrong name"), new CancellationToken());

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }
}