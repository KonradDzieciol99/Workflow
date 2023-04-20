using API.Aggregator.Models;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Services
{
    public class TaskService : BaseHttpService, ITaskService
    {
        private readonly string _tasksServiceUrl;
        public TaskService(HttpClient client, IConfiguration configuration) : base(client)
        {
            this._tasksServiceUrl = configuration.GetValue<string>("ServicesUrl:Tasks") ?? throw new ArgumentNullException(nameof(_tasksServiceUrl)); ;
        }
        public async Task<AppTaskDto> CreateTask(CreateAppTaskDto createAppTask, string token)
        {
            return await this.SendAsync<AppTaskDto>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Url = _tasksServiceUrl,
                AccessToken = token,
                Data = createAppTask
            });
        }
    }
}
