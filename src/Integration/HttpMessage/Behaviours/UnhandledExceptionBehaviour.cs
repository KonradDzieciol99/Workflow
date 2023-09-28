using MediatR;
using Microsoft.Extensions.Logging;
using HttpMessage.Authorization;
using HttpMessage.Services;
using System.Linq;
using HttpMessage.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMessage.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
            when (ex is BadRequestException
                || ex is ForbiddenException
                || ex is NotFoundException
                || ex is UnauthorizedException
                || ex is ValidationException
            )
        {
            throw;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(
                ex,
                "Request: Unhandled Exception for Request {Name} {@Request}",
                requestName,
                request
            );

            throw;
        }
    }
}
