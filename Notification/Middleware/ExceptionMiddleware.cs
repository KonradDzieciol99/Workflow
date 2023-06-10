using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common.Exceptions;
using Notification.Domain.Common.Exceptions;
using System.Net;

namespace Notification.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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
        context.Response.ContentType = "application/json";

        var type = exception.GetType();

        _logger.LogError(exception, null);

        if (exception is NotificationDomainException notificationDomainException)
        {

            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.Request.Path,
                Title = exception.Message,
            };

            var statusCode = (int)HttpStatusCode.BadRequest;

            if (notificationDomainException.InnerException is not null)
            {
                problemDetails.Title = notificationDomainException.InnerException.Message;

                if (notificationDomainException.InnerException is ForbiddenAccessException)
                    statusCode = (int)HttpStatusCode.Forbidden;
                else if (notificationDomainException.InnerException is UnauthorizedException)
                    statusCode = (int)HttpStatusCode.Unauthorized;
                else if (notificationDomainException.InnerException is ValidationException validationException)
                {
                    foreach (var item in validationException.Errors)
                        problemDetails.Errors.Add(item.Key, item.Value);
                }
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        else
        {
            var message = "An error occur.Try it again.";

            if (_env.IsDevelopment())
                message = exception.Message;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
