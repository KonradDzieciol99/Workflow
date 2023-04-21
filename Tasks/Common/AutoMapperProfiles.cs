using AutoMapper;
using MessageBus.Events;
using Tasks.Entity;
using Tasks.Models.Dtos;

namespace Tasks.Common
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateAppTaskDto, AppTask>();
            CreateMap<AppTask, CreateAppTaskDto>();

            CreateMap<ProjectMember, ProjectMemberAddedEvent>();
            CreateMap<ProjectMemberAddedEvent, ProjectMember>();

        }
    }
}
