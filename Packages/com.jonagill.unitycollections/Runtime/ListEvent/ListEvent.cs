
using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityCollections
{
    /// <summary>
    /// Replacement for System.Action that is backed by a list.
    /// System.Action is immutable, and so each modification requires re-allocating the entire array of subscribers,
    /// which can be very expensive for global events or other events with many many subscribers.
    /// By backing the collection with a list instead, we can pre-allocate and re-use memory rather than constantly
    /// re-allocating the entire collection.
    /// </summary>
    public abstract class ListEventBase<T>
    {
        protected struct ModificationData
        {
            public enum Type
            {
                ADD,
                REMOVE,
                CLEAR
            }

            public Type type;
            public T action;
        }

        private int iteratingCount;
        private readonly bool clearAfterInvoke;
        protected readonly bool useTryCatch;
        protected List<T> actionList;

        public bool HasSubcribers
        {
            get { return actionList != null && actionList.Count > 0; }
        }
        
        private List<ModificationData> queuedModifications;

        protected ListEventBase(bool useTryCatch, bool clearAfterInvoke)
        {
            this.useTryCatch = useTryCatch;
            this.clearAfterInvoke = clearAfterInvoke;
        }

        protected bool BeginInvoke()
        {
            if (actionList == null)
            {
                return false;
            }

            lock (actionList)
            {
                iteratingCount++;
            }

            return true;
        }

        protected void EndInvoke()
        {
            lock (actionList)
            {
                // In case of reentrancy, only clear the list and process queued modifications
                // when the original call finishes invoking.
                if (iteratingCount == 1)
                {
                    if (clearAfterInvoke)
                    {
                        actionList.Clear();
                    }

                    ProcessQueuedModifications();
                }
                iteratingCount--;
            }
        }

        protected void ProcessQueuedModifications()
        {
            if (queuedModifications == null)
            {
                return;
            }

            // Process all the actions in the order they came in
            int count = queuedModifications.Count;
            for (int i = 0; i < count; i++)
            {
                switch (queuedModifications[i].type)
                {
                    case ModificationData.Type.ADD:
                        actionList.Add(queuedModifications[i].action);
                        break;
                    case ModificationData.Type.REMOVE:
                        actionList.Remove(queuedModifications[i].action);
                        break;
                    case ModificationData.Type.CLEAR:
                        actionList.Clear();
                        break;
                }
            }

            queuedModifications.Clear();
        }

        private static void EnsureListCreated<U>(ref List<U> list)
        {
            if (list == null)
            {
                // Use CompareExchange to ensure that only one version of the new List
                // exists. If two callers attempt to do this at the same time on different threads, only
                // one will successfully store the list in the assigned field.
                Interlocked.CompareExchange(ref list, new List<U>(), null);
            }
        }

        public void Add(T action, bool dontTrackForDebugCleanup = false)
        {
            if (action != null)
            {
                EnsureListCreated(ref actionList);

                lock (actionList)
                {
                    if (iteratingCount > 0)
                    {
                        EnsureListCreated(ref queuedModifications);
                        queuedModifications.Add(new ModificationData { type = ModificationData.Type.ADD, action = action });
                    }
                    else
                    {
                        actionList.Add(action);
                    }
                }
            }
        }

        public void Remove(T action)
        {
            if (action != null && actionList != null)
            {
                lock (actionList)
                {
                    if (iteratingCount > 0)
                    {
                        EnsureListCreated(ref queuedModifications);
                        queuedModifications.Add(new ModificationData { type = ModificationData.Type.REMOVE, action = action });
                    }
                    else
                    {
                        actionList.Remove(action);
                    }
                }
            }
        }

        public void Clear()
        {
            if (actionList != null)
            {
                lock (actionList)
                {
                    if (iteratingCount > 0)
                    {
                        EnsureListCreated(ref queuedModifications);
                        queuedModifications.Add(new ModificationData { type = ModificationData.Type.CLEAR });
                    }
                    else
                    {
                        actionList.Clear();
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    public sealed class ListEvent : ListEventBase<Action>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke()
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke();
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke();
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }
        
        public static ListEvent operator +(ListEvent listEvent, Action action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent operator -(ListEvent listEvent, Action action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }
    
    /// <inheritdoc />
    public sealed class ListEvent<T> : ListEventBase<Action<T>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }

        public static ListEvent<T> operator +(ListEvent<T> listEvent, Action<T> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T> operator -(ListEvent<T> listEvent, Action<T> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }

    /// <inheritdoc />
    public sealed class ListEvent<T, U> : ListEventBase<Action<T, U>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t, U u)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t, u);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t, u);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }

        public static ListEvent<T, U> operator +(ListEvent<T, U> listEvent, Action<T, U> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T, U> operator -(ListEvent<T, U> listEvent, Action<T, U> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }

    /// <inheritdoc />
    public sealed class ListEvent<T, U, V> : ListEventBase<Action<T, U, V>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t, U u, V v)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t, u, v);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t, u, v);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }

        public static ListEvent<T, U, V> operator +(ListEvent<T, U, V> listEvent, Action<T, U, V> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T, U, V> operator -(ListEvent<T, U, V> listEvent, Action<T, U, V> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }

    /// <inheritdoc />
    public sealed class ListEvent<T, U, V, W> : ListEventBase<Action<T, U, V, W>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t, U u, V v, W w)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t, u, v, w);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t, u, v, w);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }

        public static ListEvent<T, U, V, W> operator +(ListEvent<T, U, V, W> listEvent, Action<T, U, V, W> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T, U, V, W> operator -(ListEvent<T, U, V, W> listEvent, Action<T, U, V, W> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }

    /// <inheritdoc />
    public sealed class ListEvent<T, U, V, W, X> : ListEventBase<Action<T, U, V, W, X>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t, U u, V v, W w, X x)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t, u, v, w, x);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t, u, v, w, x);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }

        public static ListEvent<T, U, V, W, X> operator +(ListEvent<T, U, V, W, X> listEvent, Action<T, U, V, W, X> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T, U, V, W, X> operator -(ListEvent<T, U, V, W, X> listEvent, Action<T, U, V, W, X> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }
    
    /// <inheritdoc />
    public sealed class ListEvent<T, U, V, W, X, Y> : ListEventBase<Action<T, U, V, W, X, Y>>
    {
        public ListEvent(bool useTryCatch = false, bool clearAfterInvoke = false) : base(useTryCatch, clearAfterInvoke) { }

        public void Invoke(T t, U u, V v, W w, X x, Y y)
        {
            if (!BeginInvoke())
            {
                return;
            }

            try
            {
                int count = actionList.Count;
                if (useTryCatch)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            actionList[i].Invoke(t, u, v, w, x, y);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        actionList[i].Invoke(t, u, v, w, x, y);
                    }
                }
            }
            finally
            {
                EndInvoke();
            }
        }
        
        public static ListEvent<T, U, V, W, X, Y> operator +(ListEvent<T, U, V, W, X, Y> listEvent, Action<T, U, V, W, X, Y> action)
        {
            listEvent.Add(action);
            return listEvent;
        }

        public static ListEvent<T, U, V, W, X, Y> operator -(ListEvent<T, U, V, W, X, Y> listEvent, Action<T, U, V, W, X, Y> action)
        {
            listEvent.Remove(action);
            return listEvent;
        }
    }
}