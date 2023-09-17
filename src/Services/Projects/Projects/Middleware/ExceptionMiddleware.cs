using Microsoft.AspNetCore.Mvc;
using Projects.Application.Common.Exceptions;
using Projects.Domain.Common.Exceptions;
using System.Net;

namespace Projects.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly Dictionary<Type, Func<ProjectDomainException, HttpContext, Task>> _exceptionHandlers;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        this._next = next;
        this._logger = logger;
        this._env = env;
        _exceptionHandlers = new()
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(ProjectDomainException), HandleDomainException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            };
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is ProjectDomainException taskDomainException)
        {
            Type type = taskDomainException.InnerException?.GetType() ?? typeof(ProjectDomainException);
            if (_exceptionHandlers.TryGetValue(type, out var handler))
            {
                await handler.Invoke(taskDomainException, context);
                return;
            }
        }


        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = "InternalServerError",
            Title = "Unknown error",
            Detail = "An error occur.Try it again.",
            Status = StatusCodes.Status500InternalServerError
        };

        if (_env.IsDevelopment())
            message.Detail = exception.Message;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(message);

    }

    private async Task HandleUnauthorizedAccessException(ProjectDomainException exception, HttpContext context)
    {
        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(UnauthorizedAccessException),
            Title = "Unauthorized",
            Detail = exception.Message,
            Status = StatusCodes.Status401Unauthorized
        };

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(message);
        return;
    }

    private async Task HandleForbiddenAccessException(ProjectDomainException exception, HttpContext context)
    {
        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(ForbiddenAccessException),
            Title = "You have no rights to this resource",
            Detail = exception.Message,
            Status = StatusCodes.Status403Forbidden
        };

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(message);
        return;
    }

    private async Task HandleDomainException(ProjectDomainException exception, HttpContext context)
    {
        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(Exception),
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(message);
        return;
    }
    private async Task HandleValidationException(ProjectDomainException exception, HttpContext context)
    {
        var innerException = (ValidationException)exception.InnerException!;

        var message = new ValidationProblemDetails(innerException.Errors)
        {
            Instance = context.Request.Path,
            Type = nameof(ValidationException),
            Title = "Validation errors occurred",
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(message);
        return;
    }
}
