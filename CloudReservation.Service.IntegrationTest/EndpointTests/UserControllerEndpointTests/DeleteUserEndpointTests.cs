using System.Net;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using FluentAssertions;

namespace CloudReservation.IntegrationTest.EndpointTests.UserControllerEndpointTests;

public class DeleteUserEndpointTests : IntegrationTestBase
{
    public DeleteUserEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
    }
    
    [Fact]
    public async void GivenValidUserCommand_ShouldReturn200_Ok()
    {
        //Arrange
        var validName = "Test";
        var user = new Employee()
        {
            Name = validName,
            Email = "Test",
            PinCode = "694206",
            Claims = new List<EmployeeClaim>()
            {
                new()
                {
                    ClaimType = "Booking",
                    ClaimValue = "MeetingRoom"
                }
            }
        };
        
        Db.Employees.Add(user);
        await Db.SaveChangesAsync();
        
        //Act
        var result = await Client.DeleteAsync($"api/User/delete?username={validName}");
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Employees.Count().Should().Be(0);
        Db.EmployeeClaims.Count().Should().Be(0);
    }
    
    
    //TODO: try making this a theory
    [Theory]
    [InlineData("Test")]
    [InlineData("")]
    [InlineData(null)]
    public async void GivenInvalidUserCommand_ShouldReturn400_BadRequest(string? name)
    {
        //Arrange
        var user = new Employee()
        {
            Name = "CorrectName",
            Email = "Test",
            PinCode = "694206",
            Claims = new List<EmployeeClaim>()
            {
                new()
                {
                    ClaimType = "Booking",
                    ClaimValue = "MeetingRoom"
                }
            }
        };
        
        Db.Employees.Add(user);
        await Db.SaveChangesAsync();
        
        //Act
        var result = await Client.DeleteAsync($"api/User/delete?username={name}");
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}