using HttpMessage;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Web;

namespace API.Aggregator.Services
{
    public class ProjectsService : BaseHttpService, IProjectsService
    {
        private readonly string _projectServiceUrl;
        public ProjectsService(HttpClient client, IConfiguration configuration) : base(client)
        {
            this._projectServiceUrl = configuration.GetValue<string>("ServicesUrl:Projects") ?? throw new ArgumentNullException(nameof(_projectServiceUrl)); ;
        }

        public async Task<bool> CheckIfUserIsAMemberOfProject(string userId,string projectId, string token)
        {
            StringBuilder sb = new StringBuilder(_projectServiceUrl);
            sb.Append($"/api/projects/CheckIfUserIsAMemberOfProject?userId={userId}&projectId={projectId}");

            return await this.SendAsync<bool>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = sb.ToString(),
                AccessToken = token
            });
        }
    }
}
