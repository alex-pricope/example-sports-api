using System.ComponentModel.DataAnnotations;
using MyCompany.Domain.Exceptions;

namespace MyCompany.Api.Middleware;

/// <summary>
/// Middleware to centralize the error handling.
/// This is to avoid custom error handling and parsing in the controllers. 
/// </summary>
public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // call the next middleware
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Map exceptions to specific status codes
        response.StatusCode = exception switch
        {
            // Some generic exceptions
            ValidationException => StatusCodes.Status400BadRequest,
            
            // Custom exceptions
            EntityAlreadyExistsException => StatusCodes.Status409Conflict, //Post team / player
            
            // Non defined? -> 500
            _ => StatusCodes.Status500InternalServerError
        };

        // Create the error response
        var errorResponse = new ErrorDetails()
        {
            StatusCode = response.StatusCode,
            Message = GetFullExceptionMessage(exception),
            TraceId = context.TraceIdentifier
        };

        return response.WriteAsJsonAsync(errorResponse);
    }
    
    private static string GetFullExceptionMessage(Exception exception)
    {
        var messages = new List<string> { exception.Message };

        var innerException = exception.InnerException;
        while (innerException != null)
        {
            messages.Add(innerException.Message);
            innerException = innerException.InnerException;
        }

        return string.Join(" -> ", messages);
    }
}