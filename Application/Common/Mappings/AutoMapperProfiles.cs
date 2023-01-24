using Application.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Identity.Entities;

namespace WorkflowApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //CreateProjection<AppTaskAudit, AppTaskAuditDto>()
            //    .ForMember(dest => dest.UpdaterEmail, opt => opt.MapFrom(source => source.Updater.Email))
            //    .ForMember(dest => dest.EntityTitle, opt => opt.MapFrom(source => source.Entity.Title));
            
        }
    }
}
