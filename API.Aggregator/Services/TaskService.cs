using HttpMessage;

namespace API.Aggregator.Services
{
    public class TaskService : BaseHttpService, ITaskService
    {
        public TaskService(HttpClient client) : base(client)
        {
        }

    }
}
