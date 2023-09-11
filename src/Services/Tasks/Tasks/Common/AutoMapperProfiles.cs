using AutoMapper;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.Common.Models;
using Tasks.Application.IntegrationEvents;
using Tasks.Domain.Entity;

namespace Tasks.Common;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //CreateMap<CreateAppTaskDto, AppTask>();
        //CreateMap<AppTask, CreateAppTaskDto>();

        CreateMap<AppTask, AddTaskCommand>();
        CreateMap<AddTaskCommand, AppTask>();

        CreateMap<AppTaskDto, AppTask>();
        CreateMap<AppTask, AppTaskDto>();

        CreateMap<ProjectMember, ProjectMemberAddedEvent>();
        CreateMap<ProjectMemberAddedEvent, ProjectMember>();

        CreateMap<ProjectMember, ProjectMemberDto>();
        CreateMap<ProjectMemberDto, ProjectMember>();

    }
}
