using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace DotBPE.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null || action == null)
                return;

            foreach (var item in collection)
                action(item);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> range)
        {
            foreach (var r in range)
                list.Add(r);
        }

        public static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TK> selectKeyA,
            Func<TB, TK> selectKeyB,
            Func<TA, TB, TK, TR> projection,
            TA defaultA = default(TA),
            TB defaultB = default(TB),
            IEqualityComparer<TK> cmp = null)
        {
            cmp = cmp ?? EqualityComparer<TK>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = (b ?? new List<TB>()).ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join.ToList();
        }

        public static bool Contains<T>(this IEnumerable<T> enumerable, Func<T, bool> function)
        {
            var a = enumerable.FirstOrDefault(function);
            var b = default(T);
            return !Equals(a, b);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
                if (knownKeys.Add(keySelector(element)))
                    yield return element;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

        public static IEnumerable<T> AsNullIfEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return null;

            return items;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (selector(item))
                    return index;

                index++;
            }

            // not found
            return -1;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            return IndexOf(source, item, null);
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item, IEqualityComparer<TSource> itemComparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var listOfT = source as IList<TSource>;
            if (listOfT != null)
                return listOfT.IndexOf(item);

            var list = source as IList;
            if (list != null)
                return list.IndexOf(item);

            if (itemComparer == null)
                itemComparer = EqualityComparer<TSource>.Default;

            int i = 0;
            foreach (TSource possibleItem in source)
            {
                if (itemComparer.Equals(item, possibleItem))
                    return i;

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.HashSet`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.HashSet`1"/> from.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.HashSet`1"/> that contains elements from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.HashSet`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.HashSet`1"/> from.</param>
        /// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare elements.</param>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.HashSet`1"/> that contains elements from the input sequence.
        /// </returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        /// <summary>
        /// Helper method for paging objects in a given source
        /// reference:  http://stackoverflow.com/questions/2380413/paging-with-linq-for-objects
        /// author:     http://stackoverflow.com/users/921321/lukazoid
        /// </summary>
        /// <typeparam name="T">type of object in source collection</typeparam>
        /// <param name="source">source collection to be paged</param>
        /// <param name="pageSize">page size</param>
        /// <returns>a collection of sub-collections by page size</returns>
        public static IEnumerable<IEnumerable<T>> Page<T>(this IEnumerable<T> source, int pageSize)
        {
            Contract.Requires(source != null);
            Contract.Requires(pageSize > 0);
            Contract.Ensures(Contract.Result<IEnumerable<IEnumerable<T>>>() != null);

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var currentPage = new List<T>(pageSize)
                    {
                        enumerator.Current
                    };

                    while (currentPage.Count < pageSize && enumerator.MoveNext())
                    {
                        currentPage.Add(enumerator.Current);
                    }
                    yield return new ReadOnlyCollection<T>(currentPage);
                }
            }
        }
    }
}
