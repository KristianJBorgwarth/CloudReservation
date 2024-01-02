using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Extensions;
using CloudReservation.Service.Validation.SubValidators;
using FluentValidation;

namespace CloudReservation.Service.Commands.UserCommands.Delete;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(cmd => cmd.Username)
            .NotNullMessage("Username")
            .NotEmptyMessage("Username")
            .SetValidator(new UserExistsByNameValidator(userRepository));
    }
}