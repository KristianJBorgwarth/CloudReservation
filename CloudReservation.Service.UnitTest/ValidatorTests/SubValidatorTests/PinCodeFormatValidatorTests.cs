using CloudReservation.Service.Validation.SubValidators;
using FluentAssertions;

namespace CloudReservation.UnitTest.ValidatorTests.SubValidatorTests;

public class PinFormatCodeValidatorTests
{
    private readonly PinCodeFormatValidator _validator;

    public PinFormatCodeValidatorTests()
    {
        _validator = new PinCodeFormatValidator();
    }

    [Theory]
    [InlineData("895632")]
    [InlineData("123456")]
    public void GivenValidPinCode_ShouldReturnTrue(string pinCode)
    {
        //Act
        var result = _validator.Validate(pinCode);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("89563214554")]
    [InlineData("12345")]
    [InlineData("")]
    public void GivenInvalidPinCode_ShouldReturnFalseWithMessage(string pinCode)
    {
        //Act
        var result = _validator.Validate(pinCode);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GivenNullPinCode_ShouldThrowArgumentNullException()
    {
        string? pinCode = null;

        //Act
        Action act = () => _validator.Validate(pinCode);

        //Assert
        act.Should().Throw<ArgumentNullException>();
    }
}