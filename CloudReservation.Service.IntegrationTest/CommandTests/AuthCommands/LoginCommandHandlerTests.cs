using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.AuthCommands.Login;
using CloudReservation.Service.Services.HashingService;
using CloudReservation.Service.Services.TokenServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.CommandTests.AuthCommands;

public class LoginCommandHandlerTests : IntegrationTestBase
{
    private readonly LoginCommandHandler _commandHandler;

    public LoginCommandHandlerTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();

        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _commandHandler = new LoginCommandHandler(repo, jwtService);
    }

    [Fact]
    public async void GivenValidLoginCommand_ShouldGenerateToken()
    {
        //Arrange
        var validName = "Eric";
        var validPinCode = "694206";


        var user = new Employee()
        {
            Email = "Test",
            Name = validName,
            PinCode = validPinCode,
        };

        var cmd = new LoginCommand()
        {
            PinCode = validPinCode
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        //Act
        var result = await _commandHandler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull().And.NotBe("");
    }

    [Fact]
    public async void GivenLoginCommandWithInvalidPassword_ShouldReturnFalse()
    {
        //Arrange
        var validName = "Eric";
        var invalidPinCode = "123456";

        var user = new Employee()
        {
            Email = "Test",
            Name = validName,
            PinCode = "694206",
        };

        var cmd = new LoginCommand()
        {
            PinCode = invalidPinCode
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        //Act
        var result = await _commandHandler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void GivenNullLoginCommand_ShouldThrowException()
    {
        LoginCommand? cmd = null;

        //Act
        Func<Task> act = async () => await _commandHandler.Handle(cmd!, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }
}