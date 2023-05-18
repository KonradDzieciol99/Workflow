using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;
using Projects.Application.ProjectMembers.Commands;
using Projects.Application.Projects.Commands;
using Projects.Application.Projects.Queries;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System.Net.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Projects.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProjectsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("{projectId}/projectMembers")]
        public async Task<ActionResult<ProjectMemberDto>> AddMember([FromRoute] string projectId, [FromBody] AddProjectMemberCommand command)
        {
            if (projectId != command.ProjectId)
                return BadRequest();

            return Ok(await mediator.Send(command));
        }

        [HttpDelete("{projectId}/projectMembers/{projectMemberId}")]
        public async Task<IActionResult> DeleteMember([FromRoute] string projectMemberId,
                            [FromRoute] string projectId,
                            [FromBody] DeleteProjectMemberCommand command)
        {
            if (projectId != command.ProjectId && projectMemberId != command.UserId)
                return BadRequest();

            await mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{projectId}/projectMembers/{projectMemberId}")]
        public async Task<IActionResult> DeleteMember([FromRoute] string projectMemberId,
                            [FromRoute] string projectId,
                            [FromBody] UpdateProjectMemberCommand command)
        {
            if (projectId != command.ProjectId && projectMemberId != command.UserId)
                return BadRequest();

            await mediator.Send(command);

            return NoContent();
        }

        [HttpGet("{projectId}")]
        public async Task<ProjectDto> Get([FromRoute] string projectId)
        {
            return await mediator.Send(new GetProjectQuery(projectId));
        }

        [HttpGet]
        public async Task<ProjectsWithTotalCount> Get([FromQuery] GetProjectsQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectsWithTotalCount>> Create([FromBody] CreateProjectCommand command)
        {
            var result = await mediator.Send(command);

            var url = Request.Path + "/" + result.Id;

            return Created(url, result);
        }

        [HttpDelete("{projectId}")]
        public async Task<ActionResult<ProjectsWithTotalCount>> Delete([FromRoute] string projectId)
        {
            await mediator.Send(new DeleteProjectCommand(projectId));

            return NoContent();
        }
    }
}
