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

        if (exception is ValidationException appValidationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(appValidationException.Message);
        }
        //else if (exception is BadRequestException badRequestException)
        //{
        //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    await context.Response.WriteAsync(badRequestException.Message);
        //}
        else if (exception is UnauthorizedAccessException unauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(unauthorizedAccessException.Message);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(exception.Message);
        }
    }
}
