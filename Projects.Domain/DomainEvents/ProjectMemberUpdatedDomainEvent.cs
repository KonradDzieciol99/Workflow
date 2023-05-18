using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberUpdatedDomainEvent(ProjectMember Member) : INotification;
