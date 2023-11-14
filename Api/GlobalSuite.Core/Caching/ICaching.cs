using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace GlobalSuite.Core.Caching
{
     

  public  class Caching<T> where T : class
    {
        private readonly MemoryCache _cache;
        public Caching()
        {
            _cache =MemoryCache.Default;
        }

        public List<T> GetAll(string key)
        {
            if (_cache.Contains(key)) return null;
            return  _cache.Get(key) as List<T>;
        }

        public void Add(string key, object items)
        {
            if (_cache.Contains(key)) _cache.Remove(key);
            _cache.Add(key, items, new CacheItemPolicy());
        }

        public T GetorAdd(string key, T item, CacheItemPolicy policy = null)
        {
            var itemPolicy=policy ?? new CacheItemPolicy  
            {  
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60*24)
            };
            var cacheEntry=item;
            if (_cache.Contains(key)) // Look for cache key.
                cacheEntry =(T) _cache.Get(key);
            else
            {
                _cache.Add(key, item, itemPolicy);
            }

            return cacheEntry;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }

   
}