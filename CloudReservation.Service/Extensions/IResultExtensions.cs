using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace CloudReservation.Service.Extensions;

/// <summary>
/// All extension for IResult is located in this file
/// </summary>
public static class ResultExtensions
{
    private const string ProblemDetailsKey = "ProblemDetails";

    /// <summary>
    /// Converts FluentResult to an IResult with problem details and sets the status-code to 400 by default
    /// </summary>
    /// <param name="errors">the errors that will be converted to Problem Details</param>
    /// <param name="statusCode">The StatusCode you want the Problem Details to have (will be 400 by default)</param>
    /// <returns>Returns an IResult object representing either a single problem or multiple problems</returns>
    public static IResult ConvertErrorsToBadRequestResult(this IEnumerable<IError> errors, int statusCode = StatusCodes.Status400BadRequest)
    {
        var problemDetails = errors
            .Where(error => error.Metadata.ContainsKey(ProblemDetailsKey) && error.Metadata[ProblemDetailsKey] is ProblemDetails)
            .Select(error => error.Metadata[ProblemDetailsKey] as ProblemDetails)
            .ToList();

        if (!problemDetails.Any())
        {
            return Results.StatusCode(statusCode);
        }

        if (problemDetails.Count == 1)
        {
            var singleProblem = problemDetails.First()!;
            singleProblem.Status = statusCode;
            return Results.Problem(singleProblem);
        }

        return Results.Problem(
            title: "Multiple problems occurred",
            detail: "There were multiple problems occurring when processing the request. See problems list for details.",
            type: "/errors.html#multiple",
            statusCode: statusCode,
            extensions: new Dictionary<string, object> {{"Problems", problemDetails.ToArray()}}!
        );
    }
}