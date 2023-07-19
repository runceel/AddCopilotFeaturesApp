namespace BlazorApp12.AIPlugins;

public class PageInfoProvider
{
    private readonly List<PageInfo> _pages = new();

    public void AddPage(string name, string path, string description)
    {
        _pages.Add(new (name, path, description));
    }

    public IEnumerable<PageInfo> GetPages() { return _pages.AsEnumerable(); }
}

public record PageInfo(string Name, string Path, string Description);