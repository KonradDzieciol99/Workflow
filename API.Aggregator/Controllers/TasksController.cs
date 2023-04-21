using API.Aggregator.Models;
using API.Aggregator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Aggregator.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectsService _projectsService;
        private readonly string _projectServiceUrl;

        public TasksController(ITaskService taskService, IProjectsService projectsService, IConfiguration configuration)
        {
            this._taskService = taskService ?? throw new ArgumentNullException(nameof(_taskService));
            this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(_projectsService));
            this._projectServiceUrl = configuration.GetValue<string>("ServicesUrl:Projects") ?? throw new ArgumentNullException(nameof(_projectServiceUrl)); ;

        }
        [HttpPost]
        public async Task<ActionResult<AppTaskDto>> Post([FromBody] CreateAppTaskDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var token = await HttpContext.GetTokenAsync("access_token");

            if (userId is null || string.IsNullOrEmpty(token))
                return BadRequest("User cannot be identified.");

            var projectsServiceResult = await _projectsService.CheckIfUserIsAMemberOfProject(userId, request.ProjectId, token);

            if (projectsServiceResult == false)
                return Forbid("You are not a member of this project");

            var taskServiceResult = await _taskService.CreateTask(request, token);

            return Created(_projectServiceUrl, taskServiceResult);
        }
    }
}
