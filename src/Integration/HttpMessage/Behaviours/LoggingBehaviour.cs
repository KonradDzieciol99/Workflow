using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using HttpMessage.Authorization;
using HttpMessage.Services;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace HttpMessage.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : IBaseAuthorizationRequest
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.GetUserId();
        var userEmail = _currentUserService.GetUserEmail();

        _logger.LogInformation(
            "Request: {Name} {@UserId} {@userEmail} {@Request}",
            requestName,
            userId,
            userEmail,
            request
        );
        await Task.CompletedTask;
    }
}
