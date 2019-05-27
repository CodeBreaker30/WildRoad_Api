using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace WildRoad.Tools.Utilities
{
    public class RuntimeCache
    {
        private static RuntimeCache instance = null;
        private static readonly object padlock = new object();

        private RuntimeCache() { }

        public static RuntimeCache Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RuntimeCache();
                    }
                    return instance;
                }
            }
        }

        public Boolean AddItem(String Key, Object Value)
            {
                ObjectCache _cache = MemoryCache.Default;
                return _cache.Add(Key.ToUpper(), Value, DateTime.Now.AddMinutes(120));
            }

            public Object GetItem(String Key)
            {
                ObjectCache _cache = MemoryCache.Default;
                return (_cache.Get(Key.ToUpper()));
            }
            public Boolean DeleteItem(String Key)
            {
                ObjectCache _cache = MemoryCache.Default;
                if (_cache.Remove(Key.ToUpper()) == null)
                    return true;
                else
                    return false;
            }
            public Boolean ExistItem(String Key)
            {
                ObjectCache _cache = MemoryCache.Default;
                return _cache.Contains(Key.ToUpper());
            }

            public bool AddItem(string Key, object Value, int expireMinutes)
            {
                ObjectCache _cache = MemoryCache.Default;
                return _cache.Add(Key.ToUpper(), Value, DateTime.Now.AddMinutes(expireMinutes));
            }

            public bool CleanCache()
            {
                List<string> cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
                foreach (string cacheKey in cacheKeys)
                MemoryCache.Default.Remove(cacheKey);
                return true;
            }

        public MemoryCache GetAllElementsFromMemory() {
            return MemoryCache.Default;
        }
     
    }
}
