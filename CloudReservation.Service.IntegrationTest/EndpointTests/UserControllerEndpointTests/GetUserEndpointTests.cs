using System.Net;
using System.Text.Json;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;

namespace CloudReservation.IntegrationTest.EndpointTests.UserControllerEndpointTests;

public class GetUserEndpointTests : IntegrationTestBase
{
    public GetUserEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GivenUserExistsByName_ReturnsUserDto()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "TestUser",
            Email = "Test@mail.dk",
            PinCode = "1234"
        };
        
        Db.Employees.Add(user);
        await Db.SaveChangesAsync();
        
        //Act
        var response = await Client.GetAsync($"api/user/user?username={user.Name}");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseUser = JsonSerializer.Deserialize<UserDto>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseUser.Should().NotBeNull();
        responseUser.Name.Should().Be(user.Name);
        responseUser.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GivenNoUser_ReturnsNoContent()
    {
        //Arrange
        var queryName = "Test";

        //Act
        var response = await Client.GetAsync($"api/user/user?username={queryName}");
        var responseContent = await response.Content.ReadAsStringAsync();
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        responseContent.Should().BeEmpty();
    }
}