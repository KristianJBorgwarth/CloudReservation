using AutoMapper;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using FluentResults;
using MediatR;

namespace CloudReservation.Service.Queries.User.GetUsers;

public record GetUsersQuery : IRequest<Result<IEnumerable<UserDto>>>{}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);

        if (!users.Any())
        {
            return Result.Fail<IEnumerable<UserDto>>("No users found");
        }

        var userDtos = users.Select(user => _mapper.Map<UserDto>(user)).ToArray();

        return userDtos;
    }
}