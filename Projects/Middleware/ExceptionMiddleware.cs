using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Projects.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
        if (exception is BadHttpRequestException badHttpRequestException)
        {
            //var problemDetails = new ProblemDetails
            //{
            //    //Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            //    //Title = "Bad Request",
            //    //Status = 400,
            //    Detail = badHttpRequestException.Message,
            //    //Instance = context.Request.Path
            //};

            context.Response.StatusCode = badHttpRequestException.StatusCode;
            await context.Response.WriteAsJsonAsync( new {error = badHttpRequestException.Message }); 
        }
        else if (exception is Application.Common.Exceptions.ValidationException appValidationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(appValidationException.Errors);
        }
        //else if (exception is BadRequestException badRequestException)
        //{
        //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    await context.Response.WriteAsync(badRequestException.Message);
        //}
        else if (exception is UnauthorizedAccessException unauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(unauthorizedAccessException.Message);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
    }
}
