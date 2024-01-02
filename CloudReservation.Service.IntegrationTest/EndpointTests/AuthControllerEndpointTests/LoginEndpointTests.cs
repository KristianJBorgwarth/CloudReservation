using System.Net;
using System.Text;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.IntegrationTest.IntegrationUtilities;
using CloudReservation.Service.Commands.AuthCommands.Login;
using CloudReservation.Service.Services.HashingService;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using FluentAssertions;

namespace CloudReservation.IntegrationTest.EndpointTests.AuthControllerEndpointTests;

public class LoginEndpointTests : IntegrationTestBase
{
    private readonly IHashService _hashService;
    public LoginEndpointTests(IntegrationTestFactory<Program, CloudReservationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _hashService = scope.ServiceProvider.GetRequiredService<IHashService>();
    }

    [Fact]
    public async void GivenValidCredentials_ShouldReturn200OkAndJWT()
    {
        //Arrange
        //Arrange
        var validName = "Eric";
        var validPinCode = "694206";


        var user = new Employee()
        {
            Name = validName,
            Email = "Test",
            PinCode = validPinCode,
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new LoginCommand()
        {
            PinCode = validPinCode
        };

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var result = await Client.PostAsync("api/Auth/login", content);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Should().NotBeNull();
    }

    [Fact]
    public async void GivenInvalidCredentials_ShouldReturn401Unauthorized()
    {
        //Arrange
        var validName = "Eric";
        var invalidPinCode = "694206";

        var user = new Employee()
        {
            Name = validName,
            Email = "Test",
            PinCode = "654321"
        };

        Db.Employees.Add(user);
        await Db.SaveChangesAsync();

        var cmd = new LoginCommand()
        {
            PinCode = invalidPinCode
        };

        var content = new StringContent(JsonSerializer.Serialize(cmd), Encoding.UTF8, "application/json");

        //Act
        var result = await Client.PostAsync("api/Auth/login", content);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}