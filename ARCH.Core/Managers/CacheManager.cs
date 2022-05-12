using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.Core.Managers
{
    public static class CacheManager
    {
        private const string CCH = "_xcache";

        public static object Read(this IMemoryCache cache, string key)
        {
            if (!cache.TryGetValue(key, out var entry))
            {
                return null;
            }

            return entry;
        }

        public static object Write(this IMemoryCache cache, string key, object value, int minutes = 60,
            bool slidingExpiration = false)
        {
            UpdateCacheEntries(cache, key);

            if (slidingExpiration)
            {
                return cache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(minutes)));
            }

            return cache.Set(key, value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes)));
        }

        public static bool Exists(this IMemoryCache cache, string key)
        {
            return cache.TryGetValue(key, out _);
        }

        public static void Delete(this IMemoryCache cache, string key)
        {
            cache.Remove(key);
            UpdateCacheEntries(cache, key, true);
        }

        public static void DeleteAll(this IMemoryCache cache)
        {
            var keys = GetCacheEntries(cache).ToArray();

            foreach (var key in keys)
            {
                cache.Remove(key);
            }

            UpdateCacheEntries(cache, keys, true);
        }

        private static IList<string> GetCacheEntries(IMemoryCache cache)
        {
            if (!cache.TryGetValue(CCH, out List<string> items))
            {
                items = new List<string>();
            }

            return items;
        }

        private static void UpdateCacheEntries(IMemoryCache cache, string key, bool remove = false)
        {
            UpdateCacheEntries(cache, new[] { key }, remove);
        }

        private static void UpdateCacheEntries(IMemoryCache cache, string[] keys, bool remove = false)
        {
            var items = GetCacheEntries(cache);

            foreach (var key in keys)
            {
                if (remove)
                {
                    if (items.Contains(key))
                    {
                        items.Remove(key);
                    }
                }
                else
                {
                    if (!items.Contains(key))
                    {
                        items.Add(key);
                    }
                }
            }

            cache.Set(CCH, items, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7)).RegisterPostEvictionCallback(CacheReset));
        }

        private static void CacheReset(object key, object value, EvictionReason reason, object state)
        {
            // TODO: App global scope cancel token needed to ensure heap is empty
        }
    }

}
