using API.Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "ApiScope")]
[ApiController]
public class DashboardAggregatorController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IProjectsService _projectsService;
    private readonly string _projectServiceUrl;

    public DashboardAggregatorController(ITaskService taskService, IProjectsService projectsService, IConfiguration configuration)
    {
        this._taskService = taskService ?? throw new ArgumentNullException(nameof(_taskService));
        this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(_projectsService));
        this._projectServiceUrl = configuration.GetValue<string>("ServicesUrl:Projects") ?? throw new ArgumentNullException(nameof(_projectServiceUrl)); ;

    }
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
}
