using CloudReservation.DAL.Repositories;
using FluentResults;
using MediatR;

namespace CloudReservation.Service.Commands.UserCommands.Delete;

public record DeleteUserCommand(string Username) : IRequest<Result<Unit>>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByNameIncludeClaimsAsync(request.Username, cancellationToken);

        await _userRepository.DeleteUserAsync(user, cancellationToken);

        return new Result<Unit>();
    }
}