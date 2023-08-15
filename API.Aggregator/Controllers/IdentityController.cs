using API.Aggregator.Models;
using API.Aggregator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IIdentityServerService _identityServerService;
    private readonly IChatService _chatService;
    private readonly IProjectsService _projectsService;

    public IdentityController(IIdentityServerService identityServerService,IChatService chatService,IProjectsService projectsService)
    {
        this._identityServerService = identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
        this._chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService)); ;
    }

    //public TasksController(ITaskService taskService, IProjectsService projectsService, IConfiguration configuration)
    //{
    //    this._taskService = taskService ?? throw new ArgumentNullException(nameof(_taskService));
    //    this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(_projectsService));
    //    this._projectServiceUrl = configuration.GetValue<string>("ServicesUrl:Projects") ?? throw new ArgumentNullException(nameof(_projectServiceUrl));
    //}

    [HttpGet("search/{email}")]
    public async Task<List<SearchedUserDto>> GetAsync([FromRoute] string email)
    {

        //var token = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization").Value;

        var token = await HttpContext.GetTokenAsync("access_token");

        var usersFound = await _identityServerService.SearchAsync(email, token);

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedUserDto>();

        var status = await _chatService.GetFriendsStatus(usersFound.Select(x => x.Id).ToList(), token);

        var SearchedUsers = usersFound.Select((x,index) => new SearchedUserDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)).ToList();

        return SearchedUsers;
    }
    [HttpGet("searchMember/{email}")]
    public async Task<List<SearchedMemberDto>> searchMember([FromRoute] string email, [FromQuery] string projectId)
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        var usersFound = await _identityServerService.SearchAsync(email, token);

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedMemberDto>();

        var status = await _projectsService.GetMembersStatuses(usersFound.Select(x => x.Id).ToList(), projectId, token);

        var SearchedMembers = usersFound.Select((x, index) => new SearchedMemberDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)).ToList();

        return SearchedMembers;
    }
}
