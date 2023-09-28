using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;

namespace API.Aggregator.Infrastructure.Services;

public class TaskService : BaseHttpService, ITaskService
{
    private readonly string _tasksServiceUrl;

    public TaskService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        _tasksServiceUrl =
            configuration.GetValue<string>("ServicesUrl:Tasks")
                ?? throw new InvalidOperationException("The expected configuration value 'ServicesUrl:Tasks' is missing.");
        ;
    }
}
