using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Update;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace CloudReservation.IntegrationTest.ValidatorTests.CommandValidatorTests;

public class UpdateUserClaimsCommandValidatorTests : IntegrationTestBase
{
    private readonly UpdateUserClaimsCommandValidator _validator;

    public UpdateUserClaimsCommandValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _validator = new UpdateUserClaimsCommandValidator(repo);
    }

    [Fact]
    public async void GivenValidCommand_ShouldReturnTrue()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "test",
            PinCode = "694209",
            Email = "test"
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserClaimsCommand()
        {
            Username = user.Name,
            Claims = new List<UserClaimsDto>()
            {
                new UserClaimsDto()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                }
            }
        };

        //Act
        var result = await _validator.ValidateAsync(command);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async void GivenInvalidUserName_ShouldReturnFalse()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "test",
            PinCode = "694209",
            Email = "test"
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserClaimsCommand()
        {
            Username = "This IS NOT VALID",
            Claims = new List<UserClaimsDto>()
            {
                new UserClaimsDto()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                }
            }
        };

        //Act
        var result = await _validator.ValidateAsync(command);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].ErrorMessage.Should().Be("Incorrect username");
    }

    [Fact]
    public async void GivenEmptyClaims_ShouldReturnFalse()
    {
        //Arrange
        var command = new UpdateUserClaimsCommand()
        {
            Username = "test",
            Claims = new List<UserClaimsDto>()
        };

        //Act
        var result = await _validator.ValidateAsync(command);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[1].ErrorMessage.Should().Be("Claims can't be empty");
    }

    [Fact]
    public async void GivenInvalidClaims_ShouldReturnFalse()
    {
        //Arrange
        var command = new UpdateUserClaimsCommand()
        {
            Username = "test",
            Claims = new List<UserClaimsDto>()
            {
                new UserClaimsDto()
                {
                    ClaimType = (ClaimType) 4,
                    ClaimValue = (ClaimValue) 9
                }
            }
        };

        //Act
        var result = await _validator.ValidateAsync(command);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[1].ErrorMessage.Should().Be("Claim type is not valid");
    }
}