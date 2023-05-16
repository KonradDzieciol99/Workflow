using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projects.Domain.Common.Models;

public abstract class BaseEntity
{
    //public int Id { get; set; }

    private readonly List<INotification> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(INotification domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
