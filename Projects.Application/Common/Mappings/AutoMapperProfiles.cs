using AutoMapper;
using MessageBus.Events;
using Projects.Application.Common.Models.Dto;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Application.Common.Mappings
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
