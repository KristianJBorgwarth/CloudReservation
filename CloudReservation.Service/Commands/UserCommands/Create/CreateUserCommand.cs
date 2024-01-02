using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Services.HashingService;
using FluentResults;
using MediatR;

namespace CloudReservation.Service.Commands.UserCommands.Create;

/// <summary>
/// Command for creating user
/// </summary>
/// <param name="CreateUserDto">Dto containing initial user data</param>
public record CreateUserCommand() : IRequest<Result<Unit>>
{
    public required string Name { get; init; }
    public required string PinCode { get; init; }
    public required string Email { get; init; }
    public List<UserClaimsDto> Claims { get; init; } = new();
};

/// <summary>
/// CommandHandler for CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handle the CreateUserCommand
    /// </summary>
    /// <param name="request">the command containing data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>FluentResult success</returns>
    public async Task<Result<Unit>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<CreateUserCommand, Employee>(request);

        foreach (var claim in request.Claims.Select(claimsDto => _mapper.Map<UserClaimsDto, EmployeeClaim>(claimsDto)))
        {
            user.Claims.Add(claim);
        }

        await _userRepository.AddUserAsync(user, cancellationToken);

        return new Result<Unit>();
    }
}