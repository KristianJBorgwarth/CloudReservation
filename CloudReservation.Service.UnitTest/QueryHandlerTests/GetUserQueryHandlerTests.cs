using AutoMapper;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Queries.User.GetUser;
using FakeItEasy;
using FluentAssertions;

namespace CloudReservation.UnitTest.QueryHandlerTests;

public class GetUserQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private GetUserQueryHandler _handler;
    
    public GetUserQueryHandlerTests()
    {
        _mapper = A.Fake<IMapper>();
        _userRepository = A.Fake<IUserRepository>();
        _handler = new GetUserQueryHandler(_userRepository, _mapper);   
    }
    
    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new Employee
        {
            Name = "test",
            PinCode = "test",
            Email = "test",
            Claims = new List<EmployeeClaim>()
        };
        
        var query = new GetUserQuery()
        {
            Username = user.Name
        };
        
        A.CallTo(() => _userRepository.GetUserByNameAsync(user.Name, A<CancellationToken>.Ignored)).Returns(user);
        A.CallTo(() => _mapper.Map<Employee, UserDto>(user)).Returns(new UserDto() { Name = user.Name, Email = user.Email });
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        A.CallTo(() => _userRepository.GetUserByNameAsync(user.Name, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_GivenNoUser_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetUserQuery()
        {
            Username = "test"
        };
        
        A.CallTo(() => _userRepository.GetUserByNameAsync(query.Username, A<CancellationToken>.Ignored)).Returns((Employee)null);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        A.CallTo(() => _userRepository.GetUserByNameAsync(query.Username, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        
        result.IsSuccess.Should().BeFalse();
    }
}