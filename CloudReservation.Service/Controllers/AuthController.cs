using System.ComponentModel.DataAnnotations;
using CloudReservation.Service.Commands.AuthCommands.Login;
using CloudReservation.Service.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CloudReservation.Service.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs in a user and returns a JWT token
    /// </summary>
    /// <param name="loginCommand">user input containing credentials</param>
    /// <returns>LoginResponse containing JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Login([FromBody][Required] LoginCommand loginCommand)
    {
        var result = await _mediator.Send(loginCommand);

        return result.IsSuccess ? Results.Ok(result.Value) : result.Errors.ConvertErrorsToBadRequestResult(StatusCodes.Status401Unauthorized);
    }
}