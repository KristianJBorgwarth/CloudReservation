using CloudReservation.DAL.Data;
using CloudReservation.Service.Extensions;
using CloudReservation.Service.Validation.SubValidators;
using FluentValidation;

namespace CloudReservation.Service.Commands.AuthCommands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(CloudReservationDbContext db)
    {
        RuleFor(cmd => cmd)
            .NotNullMessage("LoginCommand"); ;

        RuleFor(cmd => cmd.PinCode)
            .NotNullMessage("Password")
            .NotEmptyMessage("Password");

    }
}