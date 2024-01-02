using FluentValidation;

namespace CloudReservation.Service.Validation.SubValidators;

public class EmailFormatValidator : AbstractValidator<string>
{
    public EmailFormatValidator()
    {
        RuleFor(email => email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid")
            .Must(IsValidNovaxEmail)
            .WithMessage("Email must be a NOVAX email");
    }
    
    private static bool IsValidNovaxEmail(string email)
    {
        return email.ToLower().EndsWith("@novax.dk");
    }
}