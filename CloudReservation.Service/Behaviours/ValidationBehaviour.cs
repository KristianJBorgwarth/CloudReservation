using CloudReservation.Service.Extensions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CloudReservation.Service.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    readonly string _requestPath;
    readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IHttpContextAccessor httpContextAccessor, IValidator<TRequest>? validator = null)
    {
        _validator = validator;
        _requestPath = httpContextAccessor.HttpContext?.Request.Path ?? string.Empty;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator is null) return await next();

        var validationResult = await _validator.ValidateAsync(request, options=> options.IncludeRuleSets(), cancellationToken);

        if (validationResult.IsValid is false)
            return GenerateErrorResult(validationResult);

        return await next();
    }

    TResponse GenerateErrorResult(ValidationResult validationResult)
    {
        var result = new TResponse();
        var problemDetailsList = validationResult.Errors.Select(failure => failure.ConvertToProblemDetails());

        foreach (var problem in problemDetailsList)
            result.Reasons.Add(new Error(problem.Title).WithMetadata("ProblemDetails", problem));

        return result;
    }
}