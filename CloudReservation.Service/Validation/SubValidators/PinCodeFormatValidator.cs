using System.Text.RegularExpressions;
using FluentValidation;

namespace CloudReservation.Service.Validation.SubValidators;

public class PinCodeFormatValidator : AbstractValidator<string?>
{
    /// <summary>
    /// Validates the format of the pin code.
    /// </summary>
    public PinCodeFormatValidator()
    {
        RuleFor(pin=> pin)
            .Must(IsValidFormat)
            .WithMessage("Pin code must be 6 digits long");
    }

    /// <summary>
    /// Regex to validate the format of the pin code.
    /// </summary>
    /// <param name="pin">pin code under validation</param>
    /// <returns>bool representing validity</returns>
    private static bool IsValidFormat(string pin)
    {
        return Regex.IsMatch(pin, @"^\d{6}$");
    }
}