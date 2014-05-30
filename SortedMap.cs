/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;

namespace PeterO {
    /// <summary>A dictionary sorted by key. It is here because the Portable
    /// Class Library subset used by CBOR doesn't include the System.Collections.Generic.SortedDictionary
    /// class.</summary>
    /// <typeparam name='T1'>The type of each key.</typeparam>
    /// <typeparam name='T2'>The type of each value.</typeparam>
  internal class SortedMap<T1, T2> : IDictionary<T1, T2> {
    private RedBlackTree<KeyValuePair<T1, T2>> tree;

    private static IComparer<KeyValuePair<T1, T2>> comp = new KeyComparer();

    private sealed class KeyComparer : IComparer<KeyValuePair<T1, T2>> {
      private static IComparer<T1> keyComp = Comparer<T1>.Default;

      public int Compare(KeyValuePair<T1, T2> x, KeyValuePair<T1, T2> y) {
        return keyComp.Compare(x.Key, y.Key);
      }
    }

    public SortedMap() {
      this.tree = new RedBlackTree<KeyValuePair<T1, T2>>(comp);
    }

    public SortedMap(IDictionary<T1, T2> mapA) {
      this.tree = new RedBlackTree<KeyValuePair<T1, T2>>(comp);
      foreach (var item in mapA) {
        this.tree.AddOverwrite(item);
      }
    }

    /// <summary>Adds two T1 objects.</summary>
    /// <param name='key'>A T1 object.</param>
    /// <param name='value'>A T2 object.</param>
    public void Add(T1 key, T2 value) {
      if (!this.tree.AddIfMissing(new KeyValuePair<T1, T2>(key, value))) {
        throw new InvalidOperationException("Key already exists");
      }
    }

    public bool ContainsKey(T1 key) {
      return this.tree.Contains(new KeyValuePair<T1, T2>(key, default(T2)));
    }

    public ICollection<T1> Keys {
      get {
        var list = new List<T1>();
        foreach (var item in this.tree) {
          list.Add(item.Key);
        }
        return list;
      }
    }

    public bool Remove(T1 key) {
      return this.tree.Remove(new KeyValuePair<T1, T2>(key, default(T2)));
    }

    public bool TryGetValue(T1 key, out T2 value) {
      KeyValuePair<T1, T2> kvp;
      if (this.tree.Find(new KeyValuePair<T1, T2>(key, default(T2)), out kvp)) {
        value = kvp.Value;
        return true;
      } else {
        value = default(T2);
        return false;
      }
    }

    public ICollection<T2> Values {
      get {
        var list = new List<T2>();
        foreach (var item in this.tree) {
          list.Add(item.Value);
        }
        return list;
      }
    }

    public T2 this[T1 key] {
      get {
        KeyValuePair<T1, T2> kvp;
        if (this.tree.Find(new KeyValuePair<T1, T2>(key, default(T2)), out kvp)) {
          return kvp.Value;
        } else {
          throw new KeyNotFoundException("Key not found: " + key);
        }
      }

      set {
        this.tree.AddOverwrite(new KeyValuePair<T1, T2>(key, value));
      }
    }

    public void Add(KeyValuePair<T1, T2> item) {
      this.tree.Add(item);
    }

    public void Clear() {
      this.tree.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item) {
      return this.tree.Contains(item);
    }

    /// <param name='array'>A KeyValuePair[] object.</param>
    /// <param name='arrayIndex'>A 32-bit signed integer.</param>
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) {
      this.tree.CopyTo(array, arrayIndex);
    }

    public int Count {
      get {
        return this.tree.Count;
      }
    }

    /// <summary>Gets a value indicating whether this map is read-only.</summary>
    /// <value>Always false.</value>
    public bool IsReadOnly {
      get {
        return false;
      }
    }

    public bool Remove(KeyValuePair<T1, T2> item) {
      return this.tree.Remove(item);
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() {
      return this.tree.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.tree.GetEnumerator();
    }
  }
}
