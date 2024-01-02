using System.Text.RegularExpressions;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace CloudReservation.Service.Validation.SubValidators;

public class UsernameValidator : AbstractValidator<string>
{
    private readonly IUserRepository _userRepository;
    
    /// <summary>
    /// Validates the existence and format of a username
    /// </summary>
    /// <param name="db"></param>
    public UsernameValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(username => username)
            .Must(IsValidFormat)
            .WithMessage("Username must be between 2 and 6 characters and only contain letters")
            .Must(x => !UsernameExists(x))
            .WithMessage("Username already exists");
    }

    /// <summary>
    /// Validates that the username does not already exist in the database
    /// </summary>
    /// <param name="username">username under validation</param>
    /// <returns>bool representing existence</returns>
    private bool UsernameExists(string username)
    {
        var user = _userRepository.GetUserByName(username);

        return user is not null;
    }

    /// <summary>
    /// Validates that the username is between 2 and 6 characters long and only contains letters
    /// </summary>
    /// <param name="username">username under validation</param>
    /// <returns>bool representing validity</returns>
    private static bool IsValidFormat(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z]{2,6}$");
    }
}