using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Projects.Application.Common.ServiceInterfaces;
using System.Net;
using System.Security.Claims;

namespace Projects.Application.Requests
{
    record class GetProjectsRequest([FromServices] IUnitOfWork unitOfWork,
                                     [FromServices] IMapper mapper,
                                     ClaimsPrincipal user);


}
