#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityCollections
{
    /// <summary>
    /// Wrapper around Unity's HashSetPool API that implements IDisposable,
    /// allowing you to construct and return pooled collections
    /// using the standard C# `using` constructor.
    /// </summary>
    public class PooledHashSet<T> : ISet<T>, IReadOnlyCollection<T>, IDisposable
    {
        private HashSet<T> _set;
        public HashSet<T> BackingSet => _set;

        public int Count => _set.Count;
        public bool IsReadOnly => ((ISet<T>) _set).IsReadOnly;

        public PooledHashSet()
        {
            _set = HashSetPool<T>.Get();
        }
        
        public PooledHashSet(IEnumerable<T> collection)
        {
            _set = HashSetPool<T>.Get();
            foreach (var item in collection)
            {
                Add(item);
            }
        }
        
        ~PooledHashSet()
        {
            if (_set != null)
            {
                Debug.LogWarning($"PooledHashSet<{typeof(T).Name}> was finalized without being manually disposed. Returning to the pool automatically...");
                Dispose();
            }
        }
        
        public void Dispose()
        {
            if (_set != null)
            {
                HashSetPool<T>.Release( _set );
                _set = null;
            }
        }

        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => _set.Add( item );
        bool ISet<T>.Add(T item) => _set.Add( item );

        public void ExceptWith(IEnumerable<T> other) => _set.ExceptWith( other );

        public void IntersectWith(IEnumerable<T> other) => _set.IntersectWith( other );

        public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf( other );

        public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf( other );

        public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf( other );

        public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf( other );

        public bool Overlaps(IEnumerable<T> other) => _set.Overlaps( other );

        public bool SetEquals(IEnumerable<T> other) => _set.SetEquals( other );

        public void SymmetricExceptWith(IEnumerable<T> other) => _set.SymmetricExceptWith( other );

        public void UnionWith(IEnumerable<T> other) => _set.UnionWith( other );

        public void Clear() => _set.Clear();

        public bool Contains(T item) => _set.Contains( item );

        public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo( array, arrayIndex );

        public bool Remove(T item) => _set.Remove( item );
    }
}

#endif
