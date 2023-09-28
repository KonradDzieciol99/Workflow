using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HttpMessage.Authorization;

public interface IAuthorizationRequest<out TResponse>
    : IRequest<TResponse>,
        IBaseAuthorizationRequest { }

public interface IAuthorizationRequest : IBaseAuthorizationRequest, IRequest { }

public interface IBaseAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement();
}
