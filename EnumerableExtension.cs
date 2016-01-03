using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class EnumerableExtension
    {
        public static string ToCSVString<T>(this IEnumerable<T> list, string deliminator)
        {
            StringBuilder str = new StringBuilder();
            foreach (var l in list)
            {
                str.Append(l.ToString());
                str.Append(deliminator);
            }
            str.Length = Math.Max(0, str.Length - deliminator.Length);
            return str.ToString();
        }

        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> list, Func<T, IEnumerable<T>> func)
        {
            List<T> all = new List<T>(list);
            foreach (T i in list)
                all.AddRange(func(i).Descendants(func));
            return all;
        }

        public static int IndexOf<TSource>(this List<TSource> source, Func<TSource, bool> predicate)
        {
            for (int c = 0; c < source.Count; c++)
            {
                if (predicate.Invoke(source[c]))
                    return c;
            }
            return -1;
        }
    }
}
