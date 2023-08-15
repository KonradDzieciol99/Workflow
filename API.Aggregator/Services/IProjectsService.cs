using API.Aggregator.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Services;

public interface IProjectsService
{
    Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId, string token);
    Task<List<MemberStatusDto>> GetMembersStatuses(List<string> Ids, string projectId, string token);
    Task<ProjectMemberDto?> AddMember(string projectId, string token, object command);
}
