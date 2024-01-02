using System.Net;
using System.Text;
using System.Text.Json;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Models.UserModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CloudReservation.IntegrationTest.EndpointTests.UserControllerEndpointTests;

public class CreateUserEndpointTests : IntegrationTestBase
{
    public CreateUserEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {

    }

    [Fact]
    public async void GivenValidCreateDto_ShouldReturn201CreatedAndCreateUser()
    {
        //Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "Test@novax.dk",
            Name = "Test",
            PinCode = "694206",
            Claims = new List<UserClaimsDto>()
            {
                new()
                {
                    ClaimType = ClaimType.Booking,
                    ClaimValue = ClaimValue.MeetingRoom
                }
            }
        };
        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var result = await Client.PostAsync("api/User/create", content);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        Db.Employees.Count().Should().Be(1);
        Db.Employees.FirstOrDefault()!.Name.Should().Be(cmd.Name);
        Db.Employees.FirstOrDefault()!.PinCode.Should().Be(cmd.PinCode);
        Db.Employees.FirstOrDefault()!.Email.Should().Be(cmd.Email);
    }

    [Fact]
    public async void GivenNullCreateDto_ShouldReturn400BadRequestWithProblemDetails()
    {
        //Arrange
        CreateUserCommand? cmd = null;

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var result = await Client.PostAsync("api/User/create", content);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var resultContentString = await result.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var response = JsonSerializer.Deserialize<ProblemDetails>(resultContentString, options);
        response.Should().NotBeNull();
        response!.Title.Should().Be("Invalid request");
    }

    [Fact]
    public async void GivenCreatDto_UserAlreadyExists_ShouldReturn400BadRequestWithProblemDetails()
    {
        //Arrange
        var user = new Employee()
        {
            Name = "Erica",
            Email = "Test",
            PinCode = "556633"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new CreateUserCommand()
        {
            Email = "Test",
            Name = user.Name,
            PinCode = "552211"
        };
        

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var result = await Client.PostAsync("api/User/create", content);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var resultContentString = await result.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var response = JsonSerializer.Deserialize<ProblemDetails>(resultContentString, options);
        response.Should().NotBeNull();
        response.Detail.Should().Be("Username already exists");
    }
}