using CleanTib.Application.Common.Caching;

namespace CleanTib.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{
    // TODO: Remove this CacheKeyService
    public string GetCacheKey(string name, object id, bool includeTenantId = true)
    {
        return null;
    }
}