using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CloudReservation.Service.Extensions;

/// <summary>
///  Static class containing extensions for FluentValidation Library
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Converts a <see cref="ValidationFailure" /> object to a <see cref="ProblemDetails" /> object.
    /// This method maps properties of the <see cref="ValidationFailure" /> to equivalent properties
    /// of a <see cref="ProblemDetails" /> object, providing a standardized representation of validation failures,
    /// as per RFC 7807, suitable for API responses.
    /// </summary>
    /// <param name="validationFailure">the object to convert</param>
    /// <param name="requestPath">Optional. The URI where the error occured</param>
    /// <returns>A <see cref="ProblemDetails" /> object containing the details of the validation failure.</returns>
    public static ProblemDetails ConvertToProblemDetails(this ValidationFailure validationFailure, string? requestPath = null)
    {
        return new()
        {
            Title = validationFailure.PropertyName,
            Detail = validationFailure.ErrorMessage,
            Type = validationFailure.ErrorCode,
            Status = validationFailure.CustomState is int state ? state : StatusCodes.Status400BadRequest,
            Instance = requestPath
        };
    }

    /// <summary>
    /// Extension method for FluentValidation's RuleBuilder to add a custom "not null" validation message.
    /// </summary>
    /// <typeparam name="T">Type of object being validated.</typeparam>
    /// <typeparam name="TProperty">Type of the property being validated.</typeparam>
    /// <param name="ruleBuilder">The current IRuleBuilder instance.</param>
    /// <param name="placeholder">A custom placeholder value to be included in the validation message.</param>
    /// <returns>An IRuleBuilderOptions instance which can be used to further customize the rule.</returns>
    public static IRuleBuilderOptions<T, TProperty> NotNullMessage<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder, string placeholder)
    {
        return ruleBuilder.NotNull().WithMessage($"{placeholder} can't be empty");
    }

    /// <summary>
    /// Extension method for FluentValidation's RuleBuilder to add a custom "not empty" validation message.
    /// </summary>
    /// <param name="ruleBuilder">The current IRuleBuilder instance</param>
    /// <param name="placeholder">A custom placeholder value to be included in the validation message</param>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of the property being validated</typeparam>
    /// <returns>An IRuleBuilderOptions instance which can be used to further customize the rule</returns>
    public static IRuleBuilderOptions<T, TProperty> NotEmptyMessage<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder, string placeholder)
    {
        return ruleBuilder.NotEmpty().WithMessage($"{placeholder} can't be empty");
    }
}