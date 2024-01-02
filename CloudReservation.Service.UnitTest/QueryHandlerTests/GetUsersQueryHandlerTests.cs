using AutoMapper;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Queries.User.GetUsers;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.QueryHandlerTests;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly IMapper _fakeMapper;
    private readonly GetUsersQueryHandler _handler;
    
    public GetUsersQueryHandlerTests()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        _fakeMapper = A.Fake<IMapper>();
        _handler = new GetUsersQueryHandler(_fakeUserRepository, _fakeMapper);
    }
    
    
    [Fact]
    public async Task Handle_GivenUsersInRepo_ShouldReturnCollectionOfUserDto()
    {
        // Arrange
        var query = new GetUsersQuery();
        var users = new Employee[]
        {
            new Employee
            {
                Name = "kehe",
                Email = "asd",
                PinCode = "898989"
            },
            
            new Employee
            {
                Name = "baha",
                Email = "asd",
                PinCode = "696969"
            }
        };
        
        A.CallTo(() => _fakeUserRepository.GetUsersAsync(A<CancellationToken>.Ignored)).Returns(users);
        A.CallTo(() => _fakeMapper.Map<UserDto>(A<Employee>.Ignored)).Returns(new UserDto() {Email = "", Name = ""});
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        A.CallTo(() => _fakeUserRepository.GetUsersAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<UserDto>(A<Employee>.Ignored)).MustHaveHappenedTwiceExactly();
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_GivenNoUsersInRepo_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetUsersQuery();
        var users = new Employee[] { };
        
        A.CallTo(() => _fakeUserRepository.GetUsersAsync(A<CancellationToken>.Ignored)).Returns(users);
        A.CallTo(() => _fakeMapper.Map<UserDto>(A<Employee>.Ignored)).Returns(new UserDto() {Email = "", Name = ""});

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        A.CallTo(() => _fakeUserRepository.GetUsersAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<UserDto>(A<Employee>.Ignored)).MustNotHaveHappened();
        result.IsSuccess.Should().BeFalse();
    }
}