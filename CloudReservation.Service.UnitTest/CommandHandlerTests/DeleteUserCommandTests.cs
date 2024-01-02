using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Commands.UserCommands.Delete;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.CommandHandlerTests;

public class DeleteUserCommandTests
{
    private readonly IUserRepository _userRepository;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var username = "testUser";
        var user = new Employee
        {
            Name = username,
            PinCode = "test",
            Email = "test",
            Claims = new List<EmployeeClaim>()
            
        };
        var command = new DeleteUserCommand(username);

        A.CallTo(() => _userRepository.GetUserByNameIncludeClaimsAsync(username, A<CancellationToken>._)).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.DeleteUserAsync(user, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        result.IsSuccess.Should().BeTrue();
    }
}
