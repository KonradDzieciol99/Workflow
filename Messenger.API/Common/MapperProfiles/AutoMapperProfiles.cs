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
            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.PhotoUrl))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.PhotoUrl));
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<FriendInvitationDto, FriendInvitation>();
            CreateMap<FriendInvitation, FriendInvitationDto>();
        }
    }
}
