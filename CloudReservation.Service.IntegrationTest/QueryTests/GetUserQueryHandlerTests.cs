using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Queries.User.GetUser;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.QueryTests;

public class GetUserQueryHandlerTests : IntegrationTestBase
{
    private readonly GetUserQueryHandler _handler;
    
    public GetUserQueryHandlerTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var mapper = provider.GetRequiredService<IMapper>();
        var repo = provider.GetRequiredService<IUserRepository>();
        _handler = new GetUserQueryHandler(repo, mapper);
    }
    
    [Fact]
    public async Task GivenUserExistsByName_ReturnsUserDto()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "TestUser",
            Email = "",
            PinCode = "1234"
        };
        
        Db.Employees.Add(user);
        await Db.SaveChangesAsync();
        
        var query = new GetUserQuery()
        {
            Username = user.Name
        };
        
        //Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(user.Name);
        result.Value.Email.Should().Be(user.Email);
    }
    
    [Fact]
    public async Task GivenNoUser_ReturnsNoContent()
    {
        //Arrange
        var queryName = "Test";
        var query = new GetUserQuery()
        {
            Username = queryName
        };
        
        //Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}