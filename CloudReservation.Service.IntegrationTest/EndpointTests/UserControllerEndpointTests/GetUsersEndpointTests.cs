using System.Net;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Models.UserModels;
using System.Text.Json;
using FluentAssertions;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace CloudReservation.IntegrationTest.EndpointTests.UserControllerEndpointTests;

public class GetUsersEndpointTests : IntegrationTestBase
{
    public GetUsersEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenUsersInDatabase_WhenGetUsersEndpointIsCalled_ThenUsersAreReturned()
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
        var response = await Client.GetAsync("/api/user/users");
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var users = JsonSerializer.Deserialize<IEnumerable<UserDto>>(content, options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        users.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GivenEmptyDatabase_WhenGetUsersEndpointIsCalled_ThenNoUsersAreReturned()
    {
        // Act
        var response = await Client.GetAsync("/api/user/users");
        
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        content.Should().BeEmpty();
    }
}