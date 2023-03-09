using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18nJsonLibrary
{
    public class I18nJsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;

        public I18nJsonStringLocalizerFactory(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            // Normal read
            //return new I18nJsonStringLocalizer();
            // Distributed Cache
            return new I18nJsonStringLocalizerCache(_cache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            // Normal read
            //return new I18nJsonStringLocalizer();
            // Distributed Cache
            return new I18nJsonStringLocalizerCache(_cache);
        }
    }
}
