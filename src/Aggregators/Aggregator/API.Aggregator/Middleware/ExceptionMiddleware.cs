using API.Aggregator.Domain.Commons.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using API.Aggregator.Application.Commons.Exceptions;

namespace API.Aggregator.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly Dictionary<
        Type,
        Func<AggregatorDomainException, HttpContext, Task>
    > _exceptionHandlers;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env
    )
    {
        _next = next;
        _logger = logger;
        _env = env;
        _exceptionHandlers = new()
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(AggregatorDomainException), HandleDomainException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(NotFoundException), HandleNotFoundException },
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
        if (exception is AggregatorDomainException aggregatorDomainException)
        {
            Type type =
                aggregatorDomainException.InnerException?.GetType()
                ?? typeof(AggregatorDomainException);
            if (_exceptionHandlers.TryGetValue(type, out var handler))
            {
                await handler.Invoke(aggregatorDomainException, context);
                return;
            }
        }

        var message = "An error occur.Try it again.";
        if (_env.IsDevelopment())
            message = exception.Message;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(message);
    }

    private async Task HandleUnauthorizedAccessException(
        AggregatorDomainException exception,
        HttpContext context
    )
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

    private async Task HandleForbiddenAccessException(
        AggregatorDomainException exception,
        HttpContext context
    )
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

    private async Task HandleDomainException(
        AggregatorDomainException exception,
        HttpContext context
    )
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

    private async Task HandleValidationException(
        AggregatorDomainException exception,
        HttpContext context
    )
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

    private async Task HandleNotFoundException(
        AggregatorDomainException exception,
        HttpContext context
    )
    {
        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(NotFoundException),
            Title = "The specified resource was not found.",
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound
        };

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(message);
        return;
    }
}
