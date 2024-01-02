using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Extensions;
using CloudReservation.Service.Validation.SubValidators;
using FluentValidation;

namespace CloudReservation.Service.Commands.UserCommands.Create;

/// <summary>
/// Validator for CreateUserCommand
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /// <summary>
    /// Runs validation rules for CreateUserCommand
    /// </summary>
    /// <param name="userRepository">DbContext used for any db validation and SubValidators</param>
    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(cmd => cmd)
            .NotNullMessage("CreateUserDto");

        RuleFor(cmd => cmd.Name)
            .NotNullMessage("Name")
            .SetValidator(new UsernameValidator(userRepository));

        // RuleFor(cmd => cmd.Email)
        //     .SetValidator(new EmailFormatValidator());

        RuleFor(cmd => cmd.PinCode)
            .NotNullMessage("PinCode")
            .SetValidator(new PinCodeFormatValidator());

        RuleForEach(cmd => cmd.Claims)
            .SetValidator(new ClaimsValidator());
    }
}