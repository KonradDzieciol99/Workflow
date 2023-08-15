﻿using MediatR;
using Microsoft.AspNetCore.Authorization;


namespace Tasks.Application.Common.Authorization;

public interface IAuthorizationRequest<out TResponse> : IRequest<TResponse>, IBaseAuthorizationRequest { }
public interface IAuthorizationRequest : IRequest, IBaseAuthorizationRequest { }
public interface IBaseAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement();
}
