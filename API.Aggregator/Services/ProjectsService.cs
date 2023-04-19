using HttpMessage;
using Microsoft.Extensions.Configuration;

namespace API.Aggregator.Services
{
    public class ProjectsService : BaseHttpService, IProjectsService
    {
        private readonly string _projectServiceUrl;
        public ProjectsService(HttpClient client, IConfiguration configuration) : base(client)
        {
            this._projectServiceUrl = configuration.GetValue<string>("ServicesUrl:Projects");
        }

        public async Task<bool> CheckIfUserIsAMemberOfProject(string userId, string token)
        {
            return await this.SendAsync<bool>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = _projectServiceUrl,
                AccessToken = token
            });
        }
    }
}
