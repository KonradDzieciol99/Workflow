using API.Aggregator.Models;
using API.Aggregator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ApiScope")]

public class IdentityController : ControllerBase
{
    private readonly IIdentityServerService _identityServerService;
    private readonly IChatService _chatService;
    private readonly IProjectsService _projectsService;

    public IdentityController(IIdentityServerService identityServerService, IChatService chatService, IProjectsService projectsService)
    {
        this._identityServerService = identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
        this._chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        this._projectsService = projectsService ?? throw new ArgumentNullException(nameof(projectsService)); ;
    }

    [HttpGet("search/{email}")]
    public async Task<List<SearchedUserDto>> GetAsync([FromRoute] string email,
                                                      [FromQuery] int take,
                                                      [FromQuery] int skip)
    {

        var token = await HttpContext.GetTokenAsync("access_token");

        var usersFound = await _identityServerService.SearchAsync(email,token, take, skip);

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedUserDto>();

        var status = await _chatService.GetFriendsStatus(usersFound.Select(x => x.Id).ToList(), token);

        var SearchedUsers = usersFound.Select((x, index) => new SearchedUserDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)).ToList();

        return SearchedUsers;
    }
    [HttpGet("searchMember/{email}")]
    public async Task<List<SearchedMemberDto>> searchMember([FromRoute] string email,
                                                            [FromQuery] string projectId,
                                                            [FromQuery] int take,
                                                            [FromQuery] int skip
                                                            )
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        var usersFound = await _identityServerService.SearchAsync(email, token,take,skip);

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedMemberDto>();

        var status = await _projectsService.GetMembersStatuses(usersFound.Select(x => x.Id).ToList(), projectId, token);

        var SearchedMembers = usersFound.Select((x, index) => new SearchedMemberDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)).ToList();

        return SearchedMembers;
    }
}
