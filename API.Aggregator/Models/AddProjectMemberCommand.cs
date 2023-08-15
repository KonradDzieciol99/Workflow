namespace API.Aggregator.Models;

public record AddProjectMemberCommand(string UserId,
                        string UserEmail,
                        string? PhotoUrl,
                        ProjectMemberType Type,
                        string ProjectId);