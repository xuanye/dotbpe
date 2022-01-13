using System.Collections.Generic;
using System.Linq;



namespace DotBPE.Baseline.Extensions
{
    public static class CollectionExtensions
    {
        public static void RemoveAll<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var i in items.ToList())
                list.Remove(i);
        }
    }
}
