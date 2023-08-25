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

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        this._next = next;
        this._logger = logger;
        this._env = env;
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

        if (exception is ProjectDomainException taskDomainException)
        {

            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.Request.Path,
                Title = exception.Message,
            };

            var statusCode = (int)HttpStatusCode.BadRequest;

            if (taskDomainException.InnerException is not null)
            {
                problemDetails.Title = taskDomainException.InnerException.Message;

                if (taskDomainException.InnerException is ForbiddenAccessException)
                    statusCode = (int)HttpStatusCode.Forbidden;
                else if (taskDomainException.InnerException is UnauthorizedException)
                    statusCode = (int)HttpStatusCode.Unauthorized;
                else if (taskDomainException.InnerException is Application.Common.Exceptions.ValidationException internalEx)
                {
                    foreach (var item in internalEx.Errors)
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
    //private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    //{
    //    context.Response.ContentType = "application/json";


    //    var type = exception.GetType();
    //    if (exception is BadHttpRequestException badHttpRequestException)
    //    {
    //        //var problemDetails = new ProblemDetails
    //        //{
    //        //    //Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    //        //    //Title = "Bad Request",
    //        //    //Status = 400,
    //        //    Detail = badHttpRequestException.Message,
    //        //    //Instance = context.Request.Path
    //        //};

    //        context.Response.StatusCode = badHttpRequestException.StatusCode;
    //        await context.Response.WriteAsJsonAsync( new {error = badHttpRequestException.Message }); 
    //    }
    //    else if (exception is Application.Common.Exceptions.ValidationException appValidationException)
    //    {
    //        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //        await context.Response.WriteAsJsonAsync(appValidationException.Errors);
    //    }
    //    //else if (exception is BadRequestException badRequestException)
    //    //{
    //    //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //    //    await context.Response.WriteAsync(badRequestException.Message);
    //    //}
    //    else if (exception is UnauthorizedAccessException unauthorizedAccessException)
    //    {
    //        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //        await context.Response.WriteAsJsonAsync(unauthorizedAccessException.Message);
    //    }
    //    else
    //    {
    //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //        await context.Response.WriteAsJsonAsync(exception.Message);
    //    }
    //}
}
