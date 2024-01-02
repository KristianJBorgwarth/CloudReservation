using AutoMapper;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Entities;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudReservation.Service.Commands.UserCommands.Update;

public record UpdateUserClaimsCommand : IRequest<Result<Unit>>
{
    public required string Username { get; init; }
    public required IEnumerable<UserClaimsDto> Claims { get; init; }
}

public class UpdateUserClaimsCommandHandler : IRequestHandler<UpdateUserClaimsCommand, Result<Unit>>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UpdateUserClaimsCommandHandler(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<Result<Unit>> Handle(UpdateUserClaimsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByNameIncludeClaimsAsync(request.Username, cancellationToken);

        var claims = request.Claims.Select(claimsDto => _mapper.Map<UserClaimsDto, EmployeeClaim>(claimsDto)).ToList();

        await _userRepository.AddClaimsToUserAsync(user, claims, cancellationToken);

        return new Result<Unit>();
    }
}