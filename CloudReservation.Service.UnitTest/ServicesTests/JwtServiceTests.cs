using CloudReservation.DAL.Entities;
using CloudReservation.Service.Services.TokenServices;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.ServicesTests;

public class JwtServiceTests
{
    private readonly JwtService _jwt;

    public JwtServiceTests()
    {
        var fakeConfigurationWrapper = A.Fake<IConfigurationWrapper>();
        A.CallTo(() => fakeConfigurationWrapper.GetJwtConfiguration<int>(ConfigurationType.TimeToLive)).Returns(1);
        A.CallTo(() => fakeConfigurationWrapper.GetJwtConfiguration<string>(ConfigurationType.Audience)).Returns("Test Audience");
        A.CallTo(() => fakeConfigurationWrapper.GetJwtConfiguration<string>(ConfigurationType.Issuer)).Returns("Test Issuer");
        A.CallTo(()=> fakeConfigurationWrapper.GetJwtConfiguration<string>(ConfigurationType.Key)).Returns("918273921873ksjdhfkjdshkjsdhf28This_Is_A_Test_KEY");

        _jwt = new JwtService(fakeConfigurationWrapper);
    }

    [Fact]
    public void GivenValidUser_ShouldGenerateJwtToken()
    {
        //Create a user class with UserClaims
        var user = new Employee()
        {
            Email = "Test",
            Name = "Eric",
            PinCode = "TestPassword",
            Claims = new List<EmployeeClaim>()
            {
                new EmployeeClaim()
                {
                    ClaimType = "TestClaimType",
                    ClaimValue = "TestClaimValue"
                }
            }
        };

        //Act
        var token = _jwt.GenerateToken(user);

        //Assert
        token.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public void GivenInvalidUserShouldThrowException()
    {
        //Arrange
        Employee employee = null;

        //Act
        Action act = () => _jwt.GenerateToken(employee);

        //Assert
        act.Should().Throw<Exception>();
    }
}