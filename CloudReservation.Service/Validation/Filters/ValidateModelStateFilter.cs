using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudReservation.Service.Validation.Filters;

public class ValidateModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid request",
            Detail = string.Join("; ", errors)
        };

        context.Result = new BadRequestObjectResult(problemDetails);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}