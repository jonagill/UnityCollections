using System.Collections.Generic;

namespace UnityCollections
{
    /// <summary>
    /// Flag that can be set from multiple sources. Often preferable to a boolean flag
    /// to prevent systems stomping all over each other. Useful for state that can
    /// be set from multiple systems, such as requesting that a game pause or that an
    /// object have its visuals disabled.
    /// </summary>
    public sealed class TokenFlag
    {
        private readonly HashSet<object> tokens = new HashSet<object>();

        public bool HasTokens => _count > 0;

        // Cache the count because HashSet.Count is surprisingly expensive to invoke repeatedly
        private int _count;
        public int Count => _count;

        public bool Add(object token)
        {
            var prevHasTokens = HasTokens;
            tokens.Add(token);
            _count = tokens.Count;

            return prevHasTokens != HasTokens;
        }

        public bool Remove(object token)
        {
            var prevHasTokens = HasTokens;
            tokens.Remove(token);
            _count = tokens.Count;

            return prevHasTokens != HasTokens;
        }

        public void Clear()
        {
            tokens.Clear();
            _count = 0;
        }

        public bool Contains(object token)
        {
            return tokens.Contains(token);
        }

        public override string ToString()
        {
            return $"TokenFlag ({_count}):\n{string.Join( "\n", tokens )}";
        }
    }
}
