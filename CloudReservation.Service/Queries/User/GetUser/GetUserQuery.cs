using AutoMapper;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Models.UserModels;
using FluentResults;
using MediatR;

namespace CloudReservation.Service.Queries.User.GetUser;

public record GetUserQuery : IRequest<Result<UserDto>>
{
    public required string Username { get; init; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userService;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByNameAsync(request.Username, cancellationToken);

        if (user == null)
        {
            return Result.Fail("");
        }

        var responseUser = _mapper.Map<UserDto>(user);

        return responseUser;
    }
}