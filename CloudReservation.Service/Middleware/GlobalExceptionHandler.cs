using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace CloudReservation.Service.Middleware;

/// <summary>
/// This Handler should be registered in the Program.cs file.
/// It should be removed in production as it exposes internal logic.
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Instantiates a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="env"></param>
    public GlobalExceptionHandler(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    /// <summary>
    /// Invokes the middleware upon a request.
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and returns a JSON response.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = JsonSerializer.Serialize(new
        {
            error = exception.Message,
            stackTrace = exception.StackTrace
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(result);
    }
}