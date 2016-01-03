using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class MemoryCacheExtension
    {
        public static void Clear(this MemoryCache cache)
        {
            cache.Select(k => k.Key).ToList().ForEach(k => cache.Remove(k));
        }
    }
}
