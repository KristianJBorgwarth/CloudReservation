using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Services.TokenServices;
using FluentResults;
using MediatR;

namespace CloudReservation.Service.Commands.AuthCommands.Login;

public record LoginResponse(string Token);

public record LoginCommand : IRequest<Result<LoginResponse>>
{
    public required string PinCode { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdPinCode(request.PinCode, cancellationToken);

        if (user == null)
            return Result.Fail<LoginResponse>("Invalid credentials");

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse(token);
    }
}