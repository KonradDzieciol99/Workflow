using MessageBus;

namespace Tasks.Application.IntegrationEvents;


public class ProjectRemovedEvent : IntegrationEvent
{
    public ProjectRemovedEvent(string projectId, string name, string iconUrl)
    {
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IconUrl = iconUrl ?? throw new ArgumentNullException(nameof(iconUrl));
    }

    public string ProjectId { get; set; }
    public string Name { get; private set; }
    public string IconUrl { get; private set; }
}
