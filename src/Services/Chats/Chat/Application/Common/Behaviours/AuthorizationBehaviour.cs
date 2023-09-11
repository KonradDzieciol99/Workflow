using Chat.Application.Common.Authorization;
using Chat.Application.Common.Exceptions;
using Chat.Domain.Common.Exceptions;
using Chat.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Common.Behaviours;

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
            if (_currentUserService.GetUser() == null)
                throw new UnauthorizedAccessException();

            foreach (var requirement in authRequirementsList)
            {
                var result = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), null, requirement);

                if (!result.Succeeded)
                    throw new ChatDomainException("You do not have access to this resource", new ForbiddenAccessException());

            }
        }
        return await next();
    }
}
