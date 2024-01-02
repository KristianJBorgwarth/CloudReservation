using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Queries.User.GetUsers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CloudReservation.IntegrationTest.QueryTests;

public class GetUsersQueryHandlerTests : IntegrationTestBase
{
    private readonly GetUsersQueryHandler _handler;
    public GetUsersQueryHandlerTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = Factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var UserRepository = scopedServices.GetRequiredService<IUserRepository>();
        var Mapper = scopedServices.GetRequiredService<IMapper>();
        _handler = new GetUsersQueryHandler(UserRepository, Mapper);
    }
    
    
    [Fact]
    public async void GivenUsersInDatabase_WhenGetUsersQueryIsCalled_ThenUsersAreReturned()
    {
        // Arrange
        var user = new Employee
        {
            Name = "Test",
            Email = "Test",
            PinCode = "Test"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
    
    [Fact]
    public async void GivenEmptyDatabase_WhenGetUsersQueryIsCalled_ThenNoUsersAreReturned()
    {
        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
    
}