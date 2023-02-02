using AutoMapper;
using Socjal.API.Dto;
using Socjal.API.Entity;

namespace Socjal.API.Common.MapperProfiles
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<MessageDto, Message>();
            CreateMap<Message, MessageDto>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<FriendInvitationDto, FriendInvitation>();
            CreateMap<FriendInvitation, FriendInvitationDto>();
        }
    }
}
