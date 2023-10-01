using API.Aggregator.Application.Common.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using HttpMessage.Services;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class ProjectsService : BaseHttpService, IProjectsService
{
    private readonly string _projectServiceUrl;

    public ProjectsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));
        

        _projectServiceUrl =
            configuration.GetValue<string>("urls:internal:projects")
                ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:projects' is missing.");
    }

    public async Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId, CancellationToken cancellationToken)
    {

        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append(
            $"/api/projects/{projectId}/projectMembers/{userId}/CheckIfUserIsAMemberOfProject"
        );

        return await SendAsync<bool>(new ApiRequest(HttpMethod.Get, sb.ToString()), cancellationToken);
    }

    public async Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append(
            $"/api/projects/{projectId}/GetMembersStatuses?usersIds={string.Join("&usersIds=", Ids)}"
        );

        return await SendAsync<List<MemberStatusDto>>(
            new ApiRequest(HttpMethod.Get, sb.ToString(), null), cancellationToken
        );
    }

    public async Task<ProjectMemberDto?> AddMember(string projectId, object command, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_projectServiceUrl);
        sb.Append($"/api/projects/{projectId}/projectMembers/addMember");

        return await SendAsync<ProjectMemberDto?>(
            new ApiRequest(HttpMethod.Post, sb.ToString(), command), cancellationToken
        );
    }
}
