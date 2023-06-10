using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Notification.Services;

namespace Notification.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
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
        var userId = _currentUserService.UserId;
        var userEmail = _currentUserService.UserEmail;

        _logger.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@userEmail} {@Request}",
            requestName, userId, userEmail, request);
    }
}
