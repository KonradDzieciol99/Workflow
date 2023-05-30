﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Tasks.Application.Common.Authorization;
using Tasks.Application.Common.Exceptions;
using Tasks.Domain.Exceptions;
using Tasks.Services;

namespace Tasks.Application.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseAuthorizationRequest
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authRequirementsList = request.GetAuthorizationRequirement();

        if (authRequirementsList.Any())
        {
            if (_currentUserService.User == null)
                throw new UnauthorizedAccessException();

            foreach (var requirement in authRequirementsList)
            {
                var result = await _authorizationService.AuthorizeAsync(_currentUserService.User, null, requirement);

                if (!result.Succeeded)
                    throw new TaskDomainException("You do not have access to this resource", new ForbiddenAccessException());
            
            }
        }
        return await next();
    }
}
