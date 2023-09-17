using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : IBaseAuthorizationRequest
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {

        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.GetUserId();
        var userEmail = _currentUserService.GetUserEmail();

        _logger.LogInformation("Request: {Name} {@UserId} {@userEmail} {@Request}", requestName, userId, userEmail, request);
    }
}
