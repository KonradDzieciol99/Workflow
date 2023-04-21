using API.Aggregator.Models;

namespace API.Aggregator.Services
{
    public interface ITaskService
    {
        Task<AppTaskDto> CreateTask(CreateAppTaskDto createAppTask, string token);
    }
}
