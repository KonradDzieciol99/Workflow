using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;

namespace API.Aggregator.Infrastructure.Services;

public class TaskService : BaseHttpService<AggregatorDomainException>, ITaskService
{
    private readonly string _tasksServiceUrl;
    public TaskService(HttpClient client, IConfiguration configuration) : base(client)
    {
        _tasksServiceUrl = configuration.GetValue<string>("ServicesUrl:Tasks") ?? throw new ArgumentNullException(nameof(configuration)); ;
    }
    public async Task<AppTaskDto> CreateTask(object createAppTask)
    {
        return await SendAsync<AppTaskDto>(new ApiRequest(HttpMethod.Post, _tasksServiceUrl, createAppTask));
    }
}
