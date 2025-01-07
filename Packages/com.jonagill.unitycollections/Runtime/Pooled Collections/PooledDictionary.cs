#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityCollections
{
    /// <summary>
    /// Wrapper around Unity's DictionaryPool API that implements IDisposable,
    /// allowing you to construct and return pooled collections
    /// using the standard C# `using` constructor.
    /// </summary>
    public class PooledDictionary<T,U> : IDictionary<T,U>, IReadOnlyDictionary<T,U>, IDisposable
    {
        private Dictionary<T,U> _dictionary;
        public Dictionary<T,U> BackingDictionary => _dictionary;

        public int Count => _dictionary.Count;
        public bool IsReadOnly => ((IDictionary<T,U>) _dictionary).IsReadOnly;
        
        public U this[T key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<T> Keys => _dictionary.Keys;

        public ICollection<U> Values => _dictionary.Values;
        IEnumerable<T> IReadOnlyDictionary<T, U>.Keys => _dictionary.Keys;

        IEnumerable<U> IReadOnlyDictionary<T, U>.Values => _dictionary.Values;
        
        public PooledDictionary()
        {
            _dictionary = DictionaryPool<T,U>.Get();
        }
        
        ~PooledDictionary()
        {
            if (_dictionary != null)
            {
                Debug.LogWarning($"PooledDictionary<{typeof(T).Name},{typeof(U).Name}> was finalized without being manually disposed. Returning to the pool automatically...");
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_dictionary != null)
            {
                DictionaryPool<T,U>.Release( _dictionary );
                _dictionary = null;
            }
        }

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public void Add(T key, U value)
        {
            _dictionary.Add( key, value );
        }

        public bool Remove(T key)
        {
            return _dictionary.Remove( key );
        }

        public bool ContainsKey(T key)
        {
            return _dictionary.ContainsKey( key );
        }

        public bool TryGetValue(T key, out U value)
        {
            return _dictionary.TryGetValue( key, out value );
        }

        void ICollection<KeyValuePair<T, U>>.Add(KeyValuePair<T, U> item)
        {
            ((ICollection<KeyValuePair<T, U>>) _dictionary).Add( item );
        }

        bool ICollection<KeyValuePair<T, U>>.Contains(KeyValuePair<T, U> item)
        {
            return ((ICollection<KeyValuePair<T, U>>) _dictionary).Contains( item );
        }

        void ICollection<KeyValuePair<T, U>>.CopyTo(KeyValuePair<T, U>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T, U>>) _dictionary).CopyTo( array, arrayIndex );
        }

        bool ICollection<KeyValuePair<T, U>>.Remove(KeyValuePair<T, U> item)
        {
            return ((ICollection<KeyValuePair<T, U>>) _dictionary).Remove( item );
        }
    }
}

#endif
