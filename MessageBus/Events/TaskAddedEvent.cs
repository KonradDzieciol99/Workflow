﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events;

public class TaskAddedEvent : IntegrationEvent
{
    public TaskAddedEvent(string id, string name, string? description, string projectId, string? taskAssigneeMemberId,/* string? taskAssigneeMemberEmail, string? taskAssigneeMemberPhotoUrl,*/ int priority, int state, DateTime dueDate, DateTime startDate, string? taskLeaderId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        TaskAssigneeMemberId = taskAssigneeMemberId;
        //TaskAssigneeMemberEmail = taskAssigneeMemberEmail;
        //TaskAssigneeMemberPhotoUrl = taskAssigneeMemberPhotoUrl;
        Priority = priority;
        State = state;
        DueDate = dueDate;
        StartDate = startDate;
        TaskLeaderId = taskLeaderId;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ProjectId { get; set; }
    public string? TaskAssigneeMemberId { get; set; }
    //public string? TaskAssigneeMemberEmail { get; set; }
    //public string? TaskAssigneeMemberPhotoUrl { get; set; }
    public int Priority { get; set; }
    public int State { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime StartDate { get; set; }
    public string? TaskLeaderId { get; set; }
}
