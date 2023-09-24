using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class ProjectsService : BaseHttpService, IProjectsService
{
    private readonly string _projectServiceUrl;

    public ProjectsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        _projectServiceUrl =
            configuration.GetValue<string>("urls:internal:projects")
            ?? throw new ArgumentNullException(nameof(configuration));
        ;
    }

    public async Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append(
            $"/api/projects/CheckIfUserIsAMemberOfProject?userId={userId}&projectId={projectId}"
        );

        return await SendAsync<bool>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }

    public async Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append(
            $"/api/projects/{projectId}/GetMembersStatuses?usersIds={string.Join("&usersIds=", Ids)}"
        );

        return await SendAsync<List<MemberStatusDto>>(
            new ApiRequest(HttpMethod.Get, sb.ToString(), null)
        );
    }

    public async Task<ProjectMemberDto?> AddMember(string projectId, object command)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/projectMembers/addMember");

        return await SendAsync<ProjectMemberDto?>(
            new ApiRequest(HttpMethod.Post, sb.ToString(), command)
        );
    }
}
