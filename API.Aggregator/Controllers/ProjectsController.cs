using API.Aggregator.Models;
using API.Aggregator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Aggregator.Controllers
{
    [Route("api/[controller]/{projectId}")]
    [ApiController]
    [Authorize(Policy = "ApiScope")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IIdentityServerService _identityServerService;

        public ProjectsController(IProjectsService projectsService, IIdentityServerService identityServerService)
        {
            this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService));
            this._identityServerService = identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
        }
        [HttpPost("projectMembers/{email}")]
        public async Task<ActionResult<ProjectMemberDto?>> AddMember([FromRoute] string projectId, [FromRoute] string email)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var usersFound = await _identityServerService.CheckIfUserExistsAsync(email, token);

            if (usersFound is null)
                return NotFound("User not found");

            var result = await _projectsService.AddMember(projectId, token, new { 
                                                                UserId = usersFound.Id,
                                                                UserEmail = usersFound.Email,
                                                                PhotoUrl= usersFound.PhotoUrl,
                                                                Type= ProjectMemberType.Member,
                                                                ProjectId= projectId
                                                                });

            return result;
        }
    }
}

//[HttpPost("{projectId}/projectMembers")]
//public async Task<ActionResult<ProjectMemberDto>> AddMember([FromRoute] string projectId, [FromBody] AddProjectMemberCommand command)
//{
//    if (projectId != command.ProjectId)
//        return BadRequest();

//    return Ok(await mediator.Send(command));
//}