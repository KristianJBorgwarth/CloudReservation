using AutoMapper.Configuration.Annotations;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.ValidatorTests.CommandValidatorTests;

public class CreateUserCommandValidatorTests : IntegrationTestBase
{

    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _validator = new CreateUserCommandValidator(repo);
    }

    [Fact]
    public void GivenValidCreateUserCommand_ShouldReturnTrue()
    {
        //Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "test@novax.dk",
            Name = "Test",
            PinCode = "694206",
            Claims = new List<UserClaimsDto>()
        };

        //Act
        var result = _validator.Validate(cmd);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    
    [Fact (Skip = "Should assert exception throw instead")]
    public void GivenInvalidCreateUserCommand_ShouldReturnFalse()
    {
        //Arrange
        CreateUserCommand? cmd = null;

        //Act
        var result = _validator.Validate(cmd!);

        //Assert
        result.IsValid.Should().BeFalse();
    }
}