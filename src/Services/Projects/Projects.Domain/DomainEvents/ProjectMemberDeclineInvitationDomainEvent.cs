using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System.Runtime.CompilerServices;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberDeclineInvitationDomainEvent(ProjectMember Member, Project Project)
    : INotification;
