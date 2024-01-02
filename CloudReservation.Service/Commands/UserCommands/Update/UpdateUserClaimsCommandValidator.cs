using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Extensions;
using CloudReservation.Service.Validation.SubValidators;
using FluentValidation;

namespace CloudReservation.Service.Commands.UserCommands.Update;

public class UpdateUserClaimsCommandValidator : AbstractValidator<UpdateUserClaimsCommand>
{
    public UpdateUserClaimsCommandValidator(IUserRepository userRepository)
    {
        RuleFor(cmd => cmd.Username)
            .NotNullMessage("Username")
            .SetValidator(new UserExistsByNameValidator(userRepository));

        RuleFor(cmd => cmd.Claims)
            .NotEmptyMessage("Claims");

        RuleForEach(cmd => cmd.Claims)
            .SetValidator(new ClaimsValidator());
    }
}