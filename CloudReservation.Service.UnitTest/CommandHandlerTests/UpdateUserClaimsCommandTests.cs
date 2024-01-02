using AutoMapper;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Commands.UserCommands.Update;
using CloudReservation.Service.Models.UserModels;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.CommandHandlerTests;

public class UpdateUserClaimsCommandTests
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly UpdateUserClaimsCommandHandler _handler;

    public UpdateUserClaimsCommandTests()
    {
        _mapper = A.Fake<IMapper>();
        _userRepository = A.Fake<IUserRepository>();
        _handler = new UpdateUserClaimsCommandHandler(_mapper, _userRepository);
    }

    [Fact]
    public async Task Handle_ShouldUpdateClaims_WhenUserExists()
    {
        // Arrange
        var username = "testUser";
        var user = new Employee
        { 
            Name= username,
            PinCode = "test",
            Email = "test",
            Claims = new List<EmployeeClaim>()
        };
        var claimsDto = new List<UserClaimsDto> {};
        
        var command = new UpdateUserClaimsCommand { Username = username, Claims = claimsDto };

        A.CallTo(() => _userRepository.GetUserByNameIncludeClaimsAsync(username, A<CancellationToken>._)).Returns(user);
        A.CallTo(() => _mapper.Map<UserClaimsDto, EmployeeClaim>(A<UserClaimsDto>._)).ReturnsLazily((UserClaimsDto dto) => new EmployeeClaim { /* Map properties from dto */ });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
