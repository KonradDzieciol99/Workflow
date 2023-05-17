using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Exceptions;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseAuthorizationRequest
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    //private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        //IIdentityService identityService,
        IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        //_identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        //if(request is IAuthorizationRequest authRequest)
        //{
            //var authRequirementsList = authRequest.GetAuthorizationRequirement();
            var authRequirementsList = request.GetAuthorizationRequirement();

            if (authRequirementsList.Any())
            {
                if (_currentUserService.User == null)
                    throw new UnauthorizedAccessException();

                foreach (var requirement in authRequirementsList)
                {
                    var result = await _authorizationService.AuthorizeAsync(_currentUserService.User,null,requirement);

                    if (!result.Succeeded)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        //}
        // User is authorized / authorization not required
        return await next();
    }
}
