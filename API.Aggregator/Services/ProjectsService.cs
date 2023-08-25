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
        StringBuilder sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/CheckIfUserIsAMemberOfProject?userId={userId}&projectId={projectId}");

        return await this.SendAsync<bool>(new ApiRequest()
        {
            HttpMethod = HttpMethod.Get,
            Url = sb.ToString(),
            AccessToken = token
        });
    }
    public async Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId, string token)
    {
        StringBuilder sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/GetMembersStatuses?usersIds={string.Join(",", Ids)}");

        return await this.SendAsync<List<MemberStatusDto>>(new ApiRequest()
        {
            HttpMethod = HttpMethod.Get,
            Url = sb.ToString(),
            AccessToken = token
        });
    }
    public async Task<ProjectMemberDto?> AddMember(string projectId, string token, object command)
    {
        StringBuilder sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/projectMembers/addMember");

        return await this.SendAsync<ProjectMemberDto?>(new ApiRequest()
        {
            HttpMethod = HttpMethod.Post,
            Url = sb.ToString(),
            AccessToken = token,
            Data = command
        });
    }
}
