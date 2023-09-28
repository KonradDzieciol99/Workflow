using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HttpMessage.Exceptions;
using System.Text.Json;

namespace HttpMessage.Middleware;

public class ExceptionMiddleware<TDomainException>
    where TDomainException : Exception
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware<TDomainException>> _logger;
    private readonly Dictionary<Type, Func<Exception, HttpContext, Task>> _exceptionHandlers;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware<TDomainException>> logger
    )
    {
        _next = next;
        _logger = logger;
        _exceptionHandlers = new()
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(TDomainException), HandleDomainException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenException), HandleForbiddenAccessException },
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
        if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handler))
        {
            await handler.Invoke(exception, context);
            return;
        }

        var message = "An error occur.Try it again.";
        //if (_env.IsDevelopment())
        //    message = exception.Message;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
    }

    private async Task HandleUnauthorizedAccessException(Exception exception, HttpContext context)
    {
        var ex = exception as UnauthorizedAccessException;

        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(UnauthorizedAccessException),
            Title = "Unauthorized",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized
        };

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        return;
    }

    private async Task HandleForbiddenAccessException(Exception exception, HttpContext context)
    {
        var ex = exception as ForbiddenException;

        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(ForbiddenException),
            Title = "You have no rights to this resource",
            Detail = ex.Message,
            Status = StatusCodes.Status403Forbidden
        };

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        return;
    }

    private async Task HandleDomainException(Exception exception, HttpContext context)
    {
        var ex = exception as TDomainException;

        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(Exception),
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        return;
    }

    private async Task HandleValidationException(Exception exception, HttpContext context)
    {
        var ex = exception as ValidationException;

        var message = new ValidationProblemDetails(ex.Errors)
        {
            Instance = context.Request.Path,
            Type = nameof(ValidationException),
            Title = "Validation errors occurred",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        return;
    }

    private async Task HandleNotFoundException(Exception exception, HttpContext context)
    {
        var ex = exception as NotFoundException;

        var message = new ProblemDetails()
        {
            Instance = context.Request.Path,
            Type = nameof(NotFoundException),
            Title = "The specified resource was not found.",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound
        };

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync(JsonSerializer.Serialize(message));
        return;
    }
}
