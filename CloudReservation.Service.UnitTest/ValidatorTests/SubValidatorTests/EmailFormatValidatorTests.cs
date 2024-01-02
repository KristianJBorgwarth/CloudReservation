using CloudReservation.Service.Validation.SubValidators;
using FluentAssertions;

namespace CloudReservation.UnitTest.ValidatorTests.SubValidatorTests;

public class EmailFormatValidatorTests
{
    private readonly EmailFormatValidator _validator;
    
    public EmailFormatValidatorTests()
    {
        _validator = new EmailFormatValidator();
    }
    
    [Fact]
    public void GivenValidEmail_ShouldReturnTrue()
    {
        //Arrange
        var email = "tester@novax.dk";
        
        //Act
        var result = _validator.Validate(email);
        
        //Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void GivenNotANovaxEmail_ShouldReturnFalse()
    {
        //Arrange
        var email = "this_is_not_a_novax_email@gmail.com";
        
        //Act
        var result = _validator.Validate(email);
        
        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].ErrorMessage.Should().Be("Email must be a NOVAX email");
    }
    
    [Fact]
    public void GivenInvalidEmailFormat_ShouldReturnFalse()
    {
        //Arrange
        var email = "ThisisInvalid";
        
        //Act
        var result = _validator.Validate(email);
        
        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].ErrorMessage.Should().Be("Email is not valid");
    }
    
    [Fact]
    public void GivenEmptyEmail_ShouldReturnFalse()
    {
        //Arrange
        var email = "";
        
        //Act
        var result = _validator.Validate(email);
        
        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].ErrorMessage.Should().Be("Email is required");
    }
}