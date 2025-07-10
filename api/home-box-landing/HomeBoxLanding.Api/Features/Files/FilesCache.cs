using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Files;

public class FilesCache
{
    private static FilesCache _instance;
    private Dictionary<Guid,string> _cache;

    public FilesCache()
    {
        _cache = new Dictionary<Guid, string>();
    }

    public static FilesCache Instance()
    {
        if (_instance == null)
            _instance = new FilesCache();

        return _instance;
    }

    public void Add(Guid linkIdentifier, string linkUrl)
    {
        _cache.Add(linkIdentifier, linkUrl);
    }

    public void Remove(Guid linkIdentifier)
    {
        _cache.Remove(linkIdentifier);
    }

    public void BustCache()
    {
        _cache = new Dictionary<Guid, string>();
    }

    public bool Has(Guid linkReference)
    {
        return _cache.ContainsKey(linkReference);
    }

    public string Get(Guid linkReference)
    {
        return _cache[linkReference];
    }
}