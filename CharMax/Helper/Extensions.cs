using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Helper
{
    public static class Extensions
    {
        public static IEnumerable<TSource> IntersectAll<TSource>(
        this IEnumerable<IEnumerable<TSource>> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break;

                var set = new HashSet<TSource>(enumerator.Current);
                while (enumerator.MoveNext())
                    set.IntersectWith(enumerator.Current);
                foreach (var item in set)
                    yield return item;
            }
        }
    }
}
