using API.Aggregator.Models;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Services;

public class ProjectsService : BaseHttpService, IProjectsService
{
    private readonly string _projectServiceUrl;
    public ProjectsService(HttpClient client, IConfiguration configuration) : base(client)
    {
        this._projectServiceUrl = configuration.GetValue<string>("urls:internal:projectsHttp") ?? throw new ArgumentNullException(nameof(_projectServiceUrl)); ;
    }

    public async Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId, string token)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/CheckIfUserIsAMemberOfProject?userId={userId}&projectId={projectId}");

        return await this.SendAsync<bool>(new ApiRequest(HttpMethod.Get, sb.ToString(), null, token));
    }
    public async Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId, string token)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/GetMembersStatuses?usersIds={string.Join("&usersIds=", Ids)}");

        return await this.SendAsync<List<MemberStatusDto>>(new ApiRequest(HttpMethod.Get, sb.ToString(), null, token));
    }
    public async Task<ProjectMemberDto?> AddMember(string projectId, string token, object command)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/projectMembers/addMember");

        return await this.SendAsync<ProjectMemberDto?>(new ApiRequest(HttpMethod.Post, sb.ToString(), command, token));
    }
}
