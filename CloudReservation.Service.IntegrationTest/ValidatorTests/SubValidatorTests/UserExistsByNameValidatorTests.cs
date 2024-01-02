using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Validation.SubValidators;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.ValidatorTests.SubValidatorTests;

public class UserExistsByNameValidatorTests : IntegrationTestBase
{
    private readonly UserExistsByNameValidator _validator;

    public UserExistsByNameValidatorTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _validator = new UserExistsByNameValidator(repo);
    }

    [Fact]
    public async void GivenValidUsername_ShouldReturnTrue()
    {
        //Arrange
        var validName = "Eric";
        var user = new Employee()
        {
            Name = validName,
            Email = "Test",
            PinCode = "123456"
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        //Act
        var result = _validator.Validate(validName);

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Count.Should().Be(0);
    }

    [Fact]
    public async void GivenInvalidUsername_ShouldReturnFalse()
    {
        //Arrange
        var invalidName = "Eric";
        var user = new Employee()
        {
            Email = "Test",
            Name = "NotEric",
            PinCode = "123456"
        };

        await Db.Employees.AddAsync(user);
        await Db.SaveChangesAsync();

        //Act
        var result = _validator.Validate(invalidName);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
    }
}