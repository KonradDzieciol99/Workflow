﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Notification.Application.Common.Authorization;
using Notification.Infrastructure.Repositories;
using Notification.Services;

namespace Notification.Application.AppNotifications.Queries;

public record GetAppNotificationsQuery(int Skip, int Take, string? OrderBy, bool? IsDescending, string? Filter, string? GroupBy, string? Search, string[]? SelectedColumns) : IAuthorizationRequest<List<Domain.Entity.AppNotification>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>();
}

public class GetAppNotificationsQueryHandler : IRequestHandler<GetAppNotificationsQuery, List<Domain.Entity.AppNotification>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetAppNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }
    public async Task<List<Domain.Entity.AppNotification>> Handle(GetAppNotificationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.AppNotificationRepository.GetAsync(_currentUserService.UserId, request);

        return result.AppNotifications;
    }
}
