using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Update;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace CloudReservation.IntegrationTest.CommandTests.UserCommandTests;

public class UpdateUserClaimsCommandTests : IntegrationTestBase
{
    private readonly UpdateUserClaimsCommandHandler _handler;

    public UpdateUserClaimsCommandTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _handler = new UpdateUserClaimsCommandHandler(mapper, repo);
    }

    [Fact]
    public async void GivenValidUpdateCommand_ShouldReturnTrueAndAddClaimsToUser()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "Test",
            PinCode = "694206",
            Email = "Test"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new UpdateUserClaimsCommand()
        {
            Username = "Test",
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
        var result = await _handler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeTrue();
        Db.EmployeeClaims.FirstOrDefault().Should().NotBeNull();
        Db.EmployeeClaims.FirstOrDefault()!.ClaimType.Should().Be(cmd.Claims.FirstOrDefault()!.ClaimType.ToString());
        Db.EmployeeClaims.FirstOrDefault()!.ClaimValue.Should().Be(cmd.Claims.FirstOrDefault()!.ClaimValue.ToString());
        Db.Employees.Include(u => u.Claims).FirstOrDefault()!.Claims.FirstOrDefault()!.ClaimType.Should().Be(cmd.Claims.FirstOrDefault()!.ClaimType.ToString());
    }

    [Fact]
    public async void GivenValidUpdateCommandWithNoClaims_ShouldReturnTrueAndNotAddClaims()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "Test",
            PinCode = "694206",
            Email = "Test"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new UpdateUserClaimsCommand()
        {
            Username = "Test",
            Claims = new List<UserClaimsDto>()
        };

        //Act
        var result = await _handler.Handle(cmd, new CancellationToken());

        //Assert
        result.IsSuccess.Should().BeTrue();
        Db.EmployeeClaims.FirstOrDefault().Should().BeNull();
        Db.Employees.Include(u => u.Claims).FirstOrDefault()!.Claims.Should().BeEmpty();
    }

    [Fact]
    public async void GivenInvalidUsername_ShouldThrowException()
    {
        //Arrange
        var cmd = new UpdateUserClaimsCommand()
        {
            Username = "Test",
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
        Func<Task> act = async () => await _handler.Handle(cmd, new CancellationToken());

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }
}