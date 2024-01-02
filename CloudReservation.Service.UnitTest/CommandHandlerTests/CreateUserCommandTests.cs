using AutoMapper;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Models.UserModels;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.CommandHandlerTests;

public class CreateUserCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mapper = A.Fake<IMapper>();
        _userRepository = A.Fake<IUserRepository>();
    }

    [Fact]
    public async Task Handle_ShouldAddUser_WhenDataIsValid()
    {
        // Arrange
        var user = new Employee
        {
            Name = "test",
            PinCode = "test",
            Email = "test",
            Claims = new List<EmployeeClaim>()
        };
        var command = new CreateUserCommand()
        {
            Name = "test",
            PinCode = "test",
            Email = "test",
            Claims = new List<UserClaimsDto>()
        };

        A.CallTo(() => _mapper.Map<CreateUserCommand, Employee>(command)).Returns(user);

        _handler = new CreateUserCommandHandler(_mapper, _userRepository);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.AddUserAsync(user, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenMappingFails()
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Name = "test",
            PinCode = "test",
            Email = "",
            Claims = new List<UserClaimsDto>()
        };

        A.CallTo(() => _mapper.Map<CreateUserCommand, Employee>(command)).Throws(new Exception());

        _handler = new CreateUserCommandHandler(_mapper, _userRepository);
        
        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();

    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRepositoryFails()
    {
        // Arrange
        var user = new Employee
        {
            Name = "test",
            PinCode = "test",
            Email = "",
            Claims = new List<EmployeeClaim>()
        };
        
        var command = new CreateUserCommand()
        {
            Name = "test",
            PinCode = "test",
            Email = "",
            Claims = new List<UserClaimsDto>()
        };

        A.CallTo(() => _mapper.Map<CreateUserCommand,Employee>(command)).Returns(user);
        
        A.CallTo(() => _userRepository.AddUserAsync(user, A<CancellationToken>.Ignored)).Throws(new Exception());
        
        _handler = new CreateUserCommandHandler(_mapper, _userRepository);

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();

    }
}