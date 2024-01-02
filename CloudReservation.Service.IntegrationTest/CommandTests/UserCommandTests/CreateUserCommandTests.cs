using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Services.HashingService;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.CommandTests.UserCommandTests;

public class CreateUserCommandTests : IntegrationTestBase
{
    private readonly CreateUserCommandHandler _cmdHandler;

    public CreateUserCommandTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _cmdHandler = new CreateUserCommandHandler(mapper, repo);
    }

    [Fact]
    public async void GivenValidCreateCommand_ShouldReturnTrueAndCreateUser()
    {
        //Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "Test",
            Name = "Test",
            PinCode = "694206",
            Claims = new List<UserClaimsDto>()
            {
                new()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                }
            }
        };


        //Act
        var result = await _cmdHandler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeTrue();
        Db.Employees.FirstOrDefault().Should().NotBeNull();
        Db.Employees.FirstOrDefault()!.Name.Should().Be(cmd.Name);
        Db.Employees.FirstOrDefault()!.PinCode.Should().Be(cmd.PinCode);
        Db.Employees.FirstOrDefault()!.Email.Should().Be(cmd.Email);
        Db.EmployeeClaims.FirstOrDefault()!.ClaimType.Should().Be(cmd.Claims.FirstOrDefault()!.ClaimType.ToString());
        Db.EmployeeClaims.FirstOrDefault()!.ClaimValue.Should().Be(cmd.Claims.FirstOrDefault()!.ClaimValue.ToString());
    }

    [Fact]
    public async void GivenValidCreateCommandWithNoClaims_ShouldReturnTrueAndCreateUser()
    {
        //Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "Test",
            Name = "Test",
            PinCode = "694206",
            Claims = new List<UserClaimsDto>()
        };

        //Act
        var result = await _cmdHandler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeTrue();
        Db.Employees.FirstOrDefault().Should().NotBeNull();
        Db.Employees.FirstOrDefault()!.Name.Should().Be(cmd.Name);
        Db.Employees.FirstOrDefault()!.PinCode.Should().Be(cmd.PinCode);
        Db.EmployeeClaims.FirstOrDefault().Should().BeNull();
    }

    [Fact]
    public async void GivenInvalidCreateCommand_ShouldThrowException()
    {
        //Arrange
        var cmd = new CreateUserCommand
        {
            Email = null,
            Name = null,
            PinCode = null
        };

        //Act
        Func<Task> act = async () => await _cmdHandler.Handle(cmd, new CancellationToken());

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }
}