using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Validation.SubValidators;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.ValidatorTests.SubValidatorTests;

public class UsernameValidatorTests : IntegrationTestBase
{
    private readonly UsernameValidator _validator;
    public UsernameValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _validator = new UsernameValidator(repo);
    }

    [Fact]
    public void GivenUsernameExists_ShouldReturnFalse()
    {
        //Arrange
        var existingName = "test";

        var user = new Employee()
        {
            Email = "Test",
            Name = existingName,
            PinCode = "123456"
        };

        Db.Employees.Add(user);
        Db.SaveChanges();

        //Act
        var result = _validator.Validate(existingName);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GivenUsernameIncorrectFormat_ShouldReturnFalse()
    {
        //Arrange
        var invalidName = "test123";

        //Act
        var result = _validator.Validate(invalidName);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GivenValidUsername_ShouldReturnTrue()
    {
        //Arrange
        var validName = "test";

        //Act
        var result = _validator.Validate(validName);

        //Assert
        result.IsValid.Should().BeTrue();
    }
}