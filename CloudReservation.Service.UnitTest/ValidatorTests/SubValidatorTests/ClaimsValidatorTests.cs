using AutoMapper.Internal.Mappers;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Validation.SubValidators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CloudReservation.UnitTest.ValidatorTests.SubValidatorTests;

public class ClaimsValidatorTests
{
    private readonly ClaimsValidator _validator;

    public ClaimsValidatorTests()
    {
        _validator = new ClaimsValidator();
    }

    [Fact]
    public void GivenValidUserClaimsDto_ShouldReturnTrue()
    {
        var claims = new UserClaimsDto
        {
            ClaimType = ClaimType.Booking,
            ClaimValue = ClaimValue.MeetingRoom
        };

        var result = _validator.TestValidate(claims);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GivenValidUserClaimsDto_ShouldReturnTrue2()
    {
        var claims = new UserClaimsDto
        {
            ClaimType = ClaimType.Booking,
        };

        var result = _validator.TestValidate(claims);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GivenInvalidUserClaimsDto_ShouldReturnFalseWithMessages()
    {
        var claims = new UserClaimsDto
        {
            ClaimType = (ClaimType) 5,
            ClaimValue = (ClaimValue) 9
        };

        var result = _validator.TestValidate(claims);

        result.IsValid.Should().BeFalse();
        result.Errors[0].ErrorMessage.Should().Be("Claim type is not valid");
        result.Errors[1].ErrorMessage.Should().Be("Claim value is not valid");
    }
}