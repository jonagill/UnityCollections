# UnityCollections
This library contains a handful of small data types, wrappers, and collections that I have found useful in Unity projects in the past.

# Contents

## ListEvent
A replacement for traditional `System.Action` and `System.Action<T>` event delegates. `System.Action` is immutable, which means the entire array of subscribers must be re-allocated every time you add or remove a subscriber. While this is cheap for events with one or two subscribers, it can become extremely expensive for global events that hundreds or even thousands of objects are subscribing to. `ListEvent` has a similar API to `System.Action` but is backed by a standard C# `List<T>`, allowing the same memory to be re-used as objects are added and removed from the collection. This drastically reduces the amount of garbage generated, which can have large performance savings depending on your usages.

## TokenFlag
`TokenFlag` provides an API for a boolean flag that can be set from multiple sources. This is extremely helpful for managing things like pause state or object visibility that you want to set from multiple different systems without them stomping each other.

## Pooled collections
`PooledList<T>`, `PooledDictionary<T>`, and `PooledHashSet<T>` are all wrappers around Unity's existing `UnityEngine.Pool` APIs (available in Unity 2020.1 and above). These wrapper types implement `IDisposable`, allowing you to allocate and return a pooled collection easily and robustly with traditional C# `using` statements. Each class can be utilized directly via the relevant collection interaces, but they all also expose the backing data via a property for cases where you need to pass them to an API that expects the underlying collection type. 

For example:

```
using PooledList<Collider> pooledColliders = new();
GetComponentsInChildren(pooledColliders.BackingList);

foreach (var childCollider in pooledColliders) 
{
   // Do a thing
}

// There is no need to manually return our list to the pool
// as we declared it via using, so it will automatically be
// disposed as soon as it falls out of scope

```