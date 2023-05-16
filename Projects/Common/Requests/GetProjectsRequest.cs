using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Projects.Models;
using Projects.Repositories;
using System.Net;
using System.Security.Claims;

namespace Projects.Common.Requests
{
    record class GetProjectsRequest ([FromServices] IUnitOfWork unitOfWork,
                                     [FromServices] IMapper mapper,
                                     ClaimsPrincipal user);
    

}
