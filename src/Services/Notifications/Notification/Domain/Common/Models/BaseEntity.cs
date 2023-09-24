namespace Notification.Domain.Common.Models;

public class BaseEntity
{
    public BaseEntity() { }

    public BaseEntity(string id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Id { get; private set; }
}
