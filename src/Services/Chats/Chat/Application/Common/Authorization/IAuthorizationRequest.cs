﻿using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Common.Authorization;

public interface IAuthorizationRequest<out TResponse>
    : IRequest<TResponse>,
        IBaseAuthorizationRequest { }

public interface IAuthorizationRequest : IBaseAuthorizationRequest, IRequest { }

public interface IBaseAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement();
}
