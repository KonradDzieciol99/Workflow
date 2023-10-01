namespace Photos.Domain.Entity;

public class AppIcon
{
    public AppIcon(string url, string name)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Url { get; }
    public string Name { get; }
}
