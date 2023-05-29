using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Projects.Application.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Common.Authorization;

public interface IAuthorizationRequest<out TResponse> : IRequest<TResponse>, IBaseAuthorizationRequest{}
public interface IAuthorizationRequest : IBaseAuthorizationRequest, IRequest{}
public interface IBaseAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement();
}
