using HttpMessage.Authorization;
using HttpMessage.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.AppNotifications.Queries;

public record GetAppNotificationsQuery(
    int Skip,
    int Take,
    string? OrderBy = null,
    bool? IsDescending = null,
    string? Filter = null,
    string? GroupBy = null,
    string? Search = null,
    string[]? SelectedColumns = null
) : IAuthorizationRequest<List<Domain.Entity.AppNotification>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}

public class GetAppNotificationsQueryHandler
    : IRequestHandler<GetAppNotificationsQuery, List<Domain.Entity.AppNotification>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetAppNotificationsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<Domain.Entity.AppNotification>> Handle(
        GetAppNotificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await _unitOfWork.AppNotificationRepository.GetAsync(
            _currentUserService.GetUserId(),
            request
        );

        return result.AppNotifications;
    }
}
