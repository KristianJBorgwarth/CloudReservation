using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Commands.AuthCommands.Login;
using CloudReservation.Service.Services.TokenServices;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.CommandHandlerTests;

public class LoginCommandHandlerTests
{
    private readonly LoginCommandHandler _commandHandler;
    private readonly IUserRepository _fakeUserRepository;
    private readonly IJwtService _fakeJwtService;
    
    public LoginCommandHandlerTests()
    {
        _fakeJwtService = A.Fake<IJwtService>();
        _fakeUserRepository = A.Fake<IUserRepository>();
        _commandHandler = new LoginCommandHandler(_fakeUserRepository, _fakeJwtService);
    }
    
    [Fact]
    public void GivenInvalidPinCode_ShouldReturnFail()
    {
        //Arrange
        var invalidPinCode = "123456";
        var cmd = new LoginCommand()
        {
            PinCode = invalidPinCode
        };
        
        A.CallTo(() => _fakeUserRepository.GetUserByIdPinCode(invalidPinCode, A<CancellationToken>._)).Returns((null as Employee));
        
        //Act
        var result = _commandHandler.Handle(cmd, new CancellationToken()).Result;
        
        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors[0].Message.Should().Be("Invalid credentials");
    }
    
    [Fact]
    public void GivenValidPinCode_ShouldReturnSuccess()
    {
        //Arrange
        var validPinCode = "123456";
        var cmd = new LoginCommand()
        {
            PinCode = validPinCode
        };
        
        var user = new Employee()
        {
            Email = "Test",
            Name = "Test",
            PinCode = validPinCode,
        };
        
        A.CallTo(() => _fakeUserRepository.GetUserByIdPinCode(validPinCode, A<CancellationToken>._)).Returns(user);
        
        //Act
        var result = _commandHandler.Handle(cmd, new CancellationToken()).Result;
        
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}