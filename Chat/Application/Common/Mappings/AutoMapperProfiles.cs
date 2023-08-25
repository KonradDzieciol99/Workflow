using AutoMapper;
using Chat.Application.Common.Models;
using Chat.Domain.Entity;

namespace Chat.Application.Common.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<FriendRequest, FriendRequestDto>();
        CreateMap<FriendRequestDto, FriendRequest>();

        CreateMap<Message, MessageDto>();
        CreateMap<MessageDto, Message>();
    }
}
