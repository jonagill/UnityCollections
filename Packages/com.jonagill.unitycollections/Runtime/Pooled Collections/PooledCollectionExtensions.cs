#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections.Generic;

namespace UnityCollections
{
    public static class PooledCollectionExtensions
    {
        /// <summary>
        /// Copies the contents of the source to a new pooled list.
        /// Make sure to catch the returned value in a variable declared with "using"
        /// or manually call Dispose() on it to return the memory to the pool!
        /// </summary>
        public static PooledList<TSource> ToPooledList<TSource>(this IEnumerable<TSource> source)
        {
            return source != null ? new PooledList<TSource>(source) : throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Copies the contents of the source to a new pooled dictionary set using the given function to select keys.
        /// Make sure to catch the returned value in a variable declared with "using"
        /// or manually call Dispose() on it to return the memory to the pool!
        /// </summary>
        public static PooledDictionary<TKey, TSource> ToPooledDictionary<TSource, TKey>(
          this IEnumerable<TSource> source,
          Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            
            var pooledDictionary = new PooledDictionary<TKey, TSource>();
            foreach (var element in source)
            {
                var key = keySelector(element);
                pooledDictionary[key] = element;
            }

            return pooledDictionary;
        }
        
        /// <summary>
        /// Copies the contents of the source to a new pooled hash set.
        /// Make sure to catch the returned value in a variable declared with "using"
        /// or manually call Dispose() on it to return the memory to the pool!
        /// </summary>
        public static PooledHashSet<TSource> ToPooledHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return source != null ? new PooledHashSet<TSource>(source) : throw new ArgumentNullException(nameof(source));
        }
    }
}

#endif
