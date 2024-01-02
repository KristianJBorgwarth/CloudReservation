using CloudReservation.Service.Models.UserModels;
using FluentValidation;

namespace CloudReservation.Service.Validation.SubValidators;

/// <summary>
/// Validates that a claim is defined in the enum.
/// </summary>
public class ClaimsValidator : AbstractValidator<UserClaimsDto>
{
    /// <summary>
    /// Runs validation rules for Claims
    /// </summary>
    public ClaimsValidator()
    {
        RuleFor(claims => claims.ClaimType)
            .IsInEnum()
            .WithMessage("Claim type is not valid");

        RuleFor(claims => claims.ClaimValue)
            .IsInEnum()
            .WithMessage("Claim value is not valid");
    }
}