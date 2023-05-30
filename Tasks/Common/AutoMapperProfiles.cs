using AutoMapper;
using MessageBus.Events;
using Tasks.Application.AppTasks.Commands;
using Tasks.Domain.Entity;
using Tasks.Models.Dtos;

namespace Tasks.Common
{
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

        }
    }
}
