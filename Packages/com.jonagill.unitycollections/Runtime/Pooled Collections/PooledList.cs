#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityCollections
{
    /// <summary>
    /// Wrapper around Unity's ListPool API that implements IDisposable,
    /// allowing you to construct and return pooled collections
    /// using the standard C# `using` constructor.
    ///
    /// Use BackingList property if you need to pass the collection into an API that requires a concrete List
    /// (such as the GetComponents method family).
    /// </summary>
    public class PooledList<T> : IList<T>, IReadOnlyList<T>, IDisposable
    {
        private List<T> _list;
        public List<T> BackingList => _list;

        public int Count => _list.Count;
        public bool IsReadOnly => ((IList<T>) _list).IsReadOnly;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public PooledList()
        {
            _list = ListPool<T>.Get();
        }

        public PooledList(IEnumerable<T> collection)
        {
            _list = ListPool<T>.Get();
            AddRange(collection);
        }

        ~PooledList()
        {
            if (_list != null)
            {
                Debug.LogWarning($"PooledList<{typeof(T).Name}> was finalized without being manually disposed. Returning to the pool automatically...");
                Dispose();
            }
        }
        
        public void Dispose()
        {
            if (_list != null)
            {
                ListPool<T>.Release(_list);
                _list = null;
            }
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => _list.Add( item );

        public void AddRange(IEnumerable<T> collection) => _list.AddRange( collection );

        public void Clear() => _list.Clear();

        public bool Contains(T item) => _list.Contains( item );

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo( array, arrayIndex );

        public bool Remove(T item) => _list.Remove( item );

        public int IndexOf(T item) => _list.IndexOf( item );

        public void Insert(int index, T item) => _list.Insert( index, item );

        public void RemoveAt(int index) => _list.RemoveAt( index );
        
        public T Find(Predicate<T> match) => _list.Find( match );
    }
}

#endif
