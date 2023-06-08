using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tasks.Application.Common.Exceptions;
using Tasks.Domain.Exceptions;
using Tasks.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FluentValidation.Results;

namespace Tasks.UnitTests.Middleware;

public class ExceptionMiddlewareTests
{

    [Fact]
    public async Task InvokeAsync_WhenForbiddenAccessExceptionIsThrown_ReturnsForbidden()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns(Microsoft.Extensions.Hosting.Environments.Development);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (innerContext) =>
        {
            throw new TaskDomainException("Test message", new ForbiddenAccessException());
        };

        var middleware = new ExceptionMiddleware(next, loggerMock.Object, envMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var stream = reader.ReadToEnd();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var response = JsonSerializer.Deserialize<ProblemDetails>(stream, options);

        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task InvokeAsync_WhenCalledCompletedTask_ReturnsOk()
    {
        //Arange
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns(Microsoft.Extensions.Hosting.Environments.Development);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (innerContext) => Task.CompletedTask;

        var middleware = new ExceptionMiddleware(next, loggerMock.Object, envMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert

        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }
    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionExceptionIsThrown_ReturnsValidationProblemDetails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns(Microsoft.Extensions.Hosting.Environments.Development);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
    
        var failures = new List<ValidationFailure>() { new ValidationFailure("testProp", "testError") };
    
        RequestDelegate next = (innerContext) =>
        {
            throw new TaskDomainException("Test message", new Tasks.Application.Common.Exceptions.ValidationException(failures));
        };

        var middleware = new ExceptionMiddleware(next, loggerMock.Object, envMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var stream = reader.ReadToEnd();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var response = JsonSerializer.Deserialize<Tasks.Application.Common.Exceptions.ValidationException>(stream, options);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.NotNull(response);
    }
}
