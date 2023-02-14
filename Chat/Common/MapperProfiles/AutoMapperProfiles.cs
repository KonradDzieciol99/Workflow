﻿using AutoMapper;
using Chat.Dto;
using Chat.Entity;

namespace Chat.Common.MapperProfiles
{
    public class AutoMapperProfiles : Profile
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