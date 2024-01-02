using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.AuthCommands.Login;
using FluentAssertions;

namespace CloudReservation.IntegrationTest.ValidatorTests.CommandValidatorTests;

public class LoginCommandValidatorTests : IntegrationTestBase
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        _validator = new LoginCommandValidator(Db);
    }

    [Fact]
    public async void GivenValidLoginCommand_ShouldReturnTrue()
    {
        var user = new Employee()
        {
            Name = "TestUser",
            Email = "Test",
            PinCode = "TestPassword"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            PinCode = "TestPassword"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
    

    [Fact]
    public void GivenInvalidLoginCommandWithEmptyValues_ShouldReturnFalse()
    {
        var command = new LoginCommand
        {
            PinCode = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}