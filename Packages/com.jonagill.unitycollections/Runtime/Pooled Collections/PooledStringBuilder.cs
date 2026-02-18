#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections.Generic;
using System.Text;
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
    public class PooledStringBuilder : IDisposable
    {
        private static readonly ObjectPool<StringBuilder> Pool = new(() => new StringBuilder(), actionOnRelease: sb => sb.Clear());
        
        private StringBuilder _builder;
        public StringBuilder BackingBuilder => _builder;
        

        public PooledStringBuilder()
        {
            _builder = Pool.Get();
        }

        public PooledStringBuilder(string text)
        {
            _builder = Pool.Get();
            _builder.Append(text);
        }

        ~PooledStringBuilder()
        {
            if (_builder != null)
            {
                Debug.LogWarning($"{nameof(PooledStringBuilder)} was finalized without being manually disposed. Returning to the pool automatically...");
                Dispose();
            }
        }
        
        public void Dispose()
        {
            if (_builder != null)
            {
                Pool.Release(_builder);
                _builder = null;
            }
        }

        public char this[int index] => _builder[index];
        public int Length => _builder.Length;
        public int Capacity => _builder.Capacity;
        public int MaxCapacity => _builder.MaxCapacity;
        public StringBuilder Append(bool value) => _builder.Append(value);
        public StringBuilder Append(byte value) => _builder.Append(value);
        public StringBuilder Append(char value) => _builder.Append(value);
        public StringBuilder Append(char value, int repeatCount) => _builder.Append(value, repeatCount);
        public StringBuilder Append(char[] value) => _builder.Append(value);
        public StringBuilder Append(char[] value, int startIndex, int charCount) => _builder.Append(value, startIndex, charCount);
        public StringBuilder Append(decimal value) => _builder.Append(value);
        public StringBuilder Append(double value) => _builder.Append(value);
        public StringBuilder Append(short value) => _builder.Append(value);
        public StringBuilder Append(int value) => _builder.Append(value);
        public StringBuilder Append(long value) => _builder.Append(value);
        public StringBuilder Append(object value) => _builder.Append(value);
        public StringBuilder Append(ReadOnlySpan<char> value) => _builder.Append(value);
        public StringBuilder Append(sbyte value) => _builder.Append(value);
        public StringBuilder Append(float value) => _builder.Append(value);
        public StringBuilder Append(string value) => _builder.Append(value);
        public StringBuilder Append(string value, int startIndex, int count) => _builder.Append(value, startIndex, count);
        public StringBuilder Append(StringBuilder value) => _builder.Append(value);
        public StringBuilder Append(StringBuilder value, int startIndex, int count) => _builder.Append(value, startIndex, count);
        public StringBuilder Append(ushort value) => _builder.Append(value);
        public StringBuilder Append(uint value) => _builder.Append(value);
        public StringBuilder Append(ulong value) => _builder.Append(value);

        public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0) => _builder.AppendFormat(provider, format, arg0);
        public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1) => _builder.AppendFormat(provider, format, arg0, arg1);
        public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2) => _builder.AppendFormat(provider, format, arg0, arg1, arg2);
        public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args) => _builder.AppendFormat(provider, format, args);
        public StringBuilder AppendFormat(string format, object arg0) => _builder.AppendFormat(format, arg0);
        public StringBuilder AppendFormat(string format, object arg0, object arg1) => _builder.AppendFormat(format, arg0, arg1);
        public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2) => _builder.AppendFormat(format, arg0, arg1, arg2);
        public StringBuilder AppendFormat(string format, params object[] args) => _builder.AppendFormat(format, args);
        public StringBuilder AppendJoin(char separator, params object[] values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendJoin(char separator, params string[] values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendJoin(string separator, params object[] values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendJoin(string separator, params string[] values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendJoin<T>(char separator, IEnumerable<T> values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendJoin<T>(string separator, IEnumerable<T> values) => _builder.AppendJoin(separator, values);
        public StringBuilder AppendLine() => _builder.AppendLine();
        public StringBuilder AppendLine(string value) => _builder.AppendLine(value);
        public StringBuilder Clear() => _builder.Clear();
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) => _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        public void CopyTo(int sourceIndex, Span<char> destination, int count) => _builder.CopyTo(sourceIndex, destination, count);
        public int EnsureCapacity(int capacity) => _builder.EnsureCapacity(capacity);
        public bool Equals(ReadOnlySpan<char> span) => _builder.Equals(span);
        public bool Equals(StringBuilder sb) => _builder.Equals(sb);
        public StringBuilder Insert(int index, bool value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, byte value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, char value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, char[] value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, char[] value, int startIndex, int charCount) => _builder.Insert(index, value, startIndex, charCount);
        public StringBuilder Insert(int index, decimal value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, double value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, short value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, int value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, long value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, object value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, ReadOnlySpan<char> value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, sbyte value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, float value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, string value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, string value, int count) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, ushort value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, uint value) => _builder.Insert(index, value);
        public StringBuilder Insert(int index, ulong value) => _builder.Insert(index, value);
        public StringBuilder Remove(int startIndex, int length) => _builder.Remove(startIndex, length);
        public StringBuilder Replace(char oldChar, char newChar) => _builder.Replace(oldChar, newChar);
        public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count) => _builder.Replace(oldChar, newChar, startIndex, count);
        public StringBuilder Replace(string oldValue, string newValue) => _builder.Replace(oldValue, newValue);
        public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count) => _builder.Replace(oldValue, newValue, startIndex, count);
        
        public override string ToString() => _builder.ToString();
    }
}

#endif
