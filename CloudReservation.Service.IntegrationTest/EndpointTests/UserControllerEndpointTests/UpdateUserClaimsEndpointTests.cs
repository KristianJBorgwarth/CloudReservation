using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Update;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CloudReservation.IntegrationTest.EndpointTests.UserControllerEndpointTests;

public class UpdateUserClaimsEndpointTests : IntegrationTestBase
{
    public UpdateUserClaimsEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
    }

    [Fact]
    public async void GivenValidRequest_ShouldUpdateUserAndReturnOk200()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "Eric",
            PinCode = "1234",
            Email = "test"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new UpdateUserClaimsCommand()
        {
            Username = "Eric",
            Claims = new List<UserClaimsDto>()
            {
                new()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                },
            },
        };

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PutAsync("api/User/update", content);

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);


        Db.Employees.Include(u => u.Claims).FirstOrDefault()!.Claims.Count.Should().Be(1);
        Db.Employees.Include(u => u.Claims).FirstOrDefault()!.Claims.FirstOrDefault()!.ClaimType.Should().Be(ClaimType.Booking.ToString());
        Db.Employees.Include(u => u.Claims).FirstOrDefault()!.Claims.FirstOrDefault()!.ClaimValue.Should().Be(ClaimValue.MeetingRoom.ToString());
    }

    [Fact]
    public async void GivenInvalidRequest_ShouldReturnBadRequest400()
    {
        //Arrange
        var cmd = new UpdateUserClaimsCommand()
        {
            Username = "Eric",
            Claims = new List<UserClaimsDto>()
            {
                new()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                },
            },
        };

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PutAsync("api/User/update", content);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}