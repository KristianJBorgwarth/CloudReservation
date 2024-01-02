using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Delete;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.ValidatorTests.CommandValidatorTests;

public class DeleteUserCommandValidatorTests : IntegrationTestBase
{
    private readonly DeleteUserCommandValidator _validator;

    public DeleteUserCommandValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _validator = new DeleteUserCommandValidator(repo);
    }

    [Fact]
    public async void GivenValidUsername_ShouldReturnTrue()
    {
        //Arrange
        var validName = "James";

        var user = new Employee()
        {
            Name = validName,
            Email = "Test",
            PinCode = "694206",
            Claims = new List<EmployeeClaim>()
            {
                new()
                {
                    ClaimType = "Booking",
                    ClaimValue = "MeetingRoom"
                }
            }
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new DeleteUserCommand(validName);

        //Act
        var result = await _validator.ValidateAsync(cmd);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("")]
    [InlineData(null)]
    public async void GivenInvalidUsername_ShouldReturnFalse(string? username)
    {
        //Arrange
        var user = new Employee()
        {
            Name = "CorrectName",
            Email = "Test",
            PinCode = "694206",
            Claims = new List<EmployeeClaim>()
            {
                new()
                {
                    ClaimType = "Booking",
                    ClaimValue = "MeetingRoom"
                }
            }
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new DeleteUserCommand(username!);

        //Act
        var result = await _validator.ValidateAsync(cmd);


        //Assert
        result.IsValid.Should().BeFalse();
    }
}