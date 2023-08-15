﻿using Chat.Application.Common.Models;
using Chat.Domain.Entity;

namespace Chat.Infrastructure.Repositories;

public interface IFriendRequestRepository
{
    void Add(FriendRequest entity);
    void AddRange(IEnumerable<FriendRequest> entities);
    Task<List<FriendRequest>> GetReceivedFriendRequestsAsync(string userId);
    Task<FriendRequest?> GetAsync(string sourceUserId, string targetUserId);
    void Remove(FriendRequest entity);
    Task<List<FriendRequest>> GetConfirmedAsync(string UserId);
    Task<bool> CheckIfTheyShareFriendRequest(string userId, string targetUserId);
    Task<List<FriendStatusDto>> CheckUsersToUserStatusAsync(string userId, List<string> userIds);
}