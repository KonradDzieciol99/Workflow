﻿using API.Aggregator.Application.Commons.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface ITaskService
{
    Task<AppTaskDto> CreateTask(object createAppTask);
}
