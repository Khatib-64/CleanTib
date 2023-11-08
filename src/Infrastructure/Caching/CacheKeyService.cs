using CleanTib.Application.Common.Caching;

namespace CleanTib.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{
    public CacheKeyService()
    {
    }

    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}