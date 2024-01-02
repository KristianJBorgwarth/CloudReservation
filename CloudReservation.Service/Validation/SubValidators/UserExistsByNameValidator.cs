using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CloudReservation.Service.Validation.SubValidators;

public class UserExistsByNameValidator : AbstractValidator<string>
{
    public UserExistsByNameValidator(IUserRepository _userRepository)
    {
        RuleFor(username => username)
            .Must(username => _userRepository.GetUserByName(username) is not null)
            .WithMessage("Incorrect username");
    }
}