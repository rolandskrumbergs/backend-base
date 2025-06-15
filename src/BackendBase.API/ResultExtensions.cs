using BackendBase.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BackendBase.API;

/// <summary>
/// Extension methods for converting Result types to ASP.NET Core IActionResult
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Converts a Result{T} to an appropriate IActionResult based on the result status
    /// </summary>
    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));

        return result.Status switch
        {
            ResultStatus.Ok => result.Value is null ? new OkResult() : new OkObjectResult(result.Value),
            ResultStatus.Created => new ObjectResult(result.Value) { StatusCode = StatusCodes.Status201Created },
            ResultStatus.NoContent => new NoContentResult(),
            ResultStatus.NotFound => CreateProblemDetails(result, StatusCodes.Status404NotFound, "Not Found"),
            ResultStatus.Unauthorized => new UnauthorizedResult(),
            ResultStatus.Invalid => CreateValidationProblemDetails(result),
            ResultStatus.Error => CreateProblemDetails(result, StatusCodes.Status500InternalServerError, "Internal Server Error"),
            _ => CreateProblemDetails(result, StatusCodes.Status500InternalServerError, "Unexpected Error")
        };
    }

    /// <summary>
    /// Converts a non-generic Result to an appropriate IActionResult based on the result status
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));

        return result.Status switch
        {
            ResultStatus.Ok => new OkResult(),
            ResultStatus.Created => new StatusCodeResult(StatusCodes.Status201Created),
            ResultStatus.NoContent => new NoContentResult(),
            ResultStatus.NotFound => CreateProblemDetails(result, StatusCodes.Status404NotFound, "Not Found"),
            ResultStatus.Unauthorized => new UnauthorizedResult(),
            ResultStatus.Invalid => CreateValidationProblemDetails(result),
            ResultStatus.Error => CreateProblemDetails(result, StatusCodes.Status500InternalServerError, "Internal Server Error"),
            _ => CreateProblemDetails(result, StatusCodes.Status500InternalServerError, "Unexpected Error")
        };
    }

    private static ObjectResult CreateProblemDetails<T>(Result<T> result, int statusCode, string title)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = result.Errors.Any() ? string.Join(", ", result.Errors) : null
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static ObjectResult CreateProblemDetails(Result result, int statusCode, string title)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = result.Errors.Any() ? string.Join(", ", result.Errors) : null
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static BadRequestObjectResult CreateValidationProblemDetails<T>(Result<T> result)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred",
            Errors = new Dictionary<string, string[]>
            {
                { "ValidationErrors", result.Errors.ToArray() }
            }
        };

        return new BadRequestObjectResult(problemDetails);
    }

    private static BadRequestObjectResult CreateValidationProblemDetails(Result result)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred",
            Errors = new Dictionary<string, string[]>
            {
                { "ValidationErrors", result.Errors.ToArray() }
            }
        };

        return new BadRequestObjectResult(problemDetails);
    }
}
