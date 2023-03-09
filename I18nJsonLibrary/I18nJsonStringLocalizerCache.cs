using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18nJsonLibrary
{
    public class I18nJsonStringLocalizerCache : I18nJsonStringLocalizer
    {
        private readonly IDistributedCache _cache;

        public I18nJsonStringLocalizerCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        override public string? FindBy(string key)
        {
            string cacheKey = CacheKey(key);
            string cacheValue = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;
            string? result = FindResourceValueBy(key);
            if (!string.IsNullOrEmpty(result)) _cache.SetString(cacheKey, result);
            return result;
        }

        private string CacheKey(string key)
        {
            return $"i18n_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
        }
    }
}
