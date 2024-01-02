using System.ComponentModel.DataAnnotations;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Commands.UserCommands.Delete;
using CloudReservation.Service.Commands.UserCommands.Update;
using CloudReservation.Service.Extensions;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Queries.User.GetUser;
using CloudReservation.Service.Queries.User.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CloudReservation.Service.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a user and corresponding claims
    /// </summary>
    /// <param name="dto">A CreateUserDto containing the user information</param>
    /// <returns>StatusCode representing the result of the operation</returns>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateUser([FromBody][Required] CreateUserCommand cmd)
    {
        var result = await _mediator.Send(cmd);

        return result.IsSuccess ? Results.Created(nameof(CreateUser), null) : result.Errors.ConvertErrorsToBadRequestResult();
    }

    /// <summary>
    /// Gets all users 
    /// </summary>
    /// <returns>A collection containing all users</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());

        return result.IsSuccess ? Results.Ok(result.Value) : Results.NoContent();
    }
    
    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="query">query containing username from query</param>
    /// <returns>A single Dto containing user</returns>
    [HttpGet("user")]
    [ProducesResponseType(typeof(UserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> GetUser([FromQuery][Required] GetUserQuery query)
    {
        var result = await _mediator.Send(query);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.NoContent();
    }
    
    
    /// <summary>
    /// Deletes user and corresponding claims by username
    /// </summary>
    /// <param name="cmd">Object containing the username for deletion</param>
    /// <returns>StatusCode representing the result of the operation</returns>
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> DeleteUser([FromQuery][Required] DeleteUserCommand cmd)
    {
        var result = await _mediator.Send(cmd);

        return result.IsSuccess ? Results.Ok() : result.Errors.ConvertErrorsToBadRequestResult();
    }

    /// <summary>
    /// Updates the users claims
    /// </summary>
    /// <param name="command">request containing claims and user id</param>
    /// <returns></returns>
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateUserClaims([FromBody][Required] UpdateUserClaimsCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess ? Results.Ok() : result.Errors.ConvertErrorsToBadRequestResult();
    }
}