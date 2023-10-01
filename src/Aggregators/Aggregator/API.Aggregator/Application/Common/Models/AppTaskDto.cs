namespace API.Aggregator.Application.Common.Models;

public class AppTaskDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required string ProjectId { get; set; }
    public string? TaskAssigneeMemberId { get; set; }
    public required Priority Priority { get; set; }
    public required State State { get; set; }
    public required DateTime DueDate { get; set; }
    public required DateTime StartDate { get; set; }
    public string? TaskLeaderId { get; set; }
}
