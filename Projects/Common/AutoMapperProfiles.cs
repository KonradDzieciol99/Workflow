using AutoMapper;
using MessageBus.Events;
using Projects.Entity;
using Projects.Models.Dto;

namespace Projects.Common
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<ProjectMember, ProjectMemberDto>();
            CreateMap<ProjectMemberDto, ProjectMember>();

            CreateMap<ProjectMemberAddedEvent, ProjectMember>();
            CreateMap<ProjectMember, ProjectMemberAddedEvent>();
            
        }
    }
}
