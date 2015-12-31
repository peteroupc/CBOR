/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;

namespace PeterO {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.SortedMap`2"]/*'/>
  internal class SortedMap<T1, T2> : IDictionary<T1, T2> {
    private readonly RedBlackTree<KeyValuePair<T1, T2>> tree;
    private static readonly IComparer<KeyValuePair<T1, T2>> ValueComp = new
      KeyComparer();

    private sealed class KeyComparer : IComparer<KeyValuePair<T1, T2>> {
      private static readonly IComparer<T1> KeyComp = Comparer<T1>.Default;

      public int Compare(KeyValuePair<T1, T2> x, KeyValuePair<T1, T2> y) {
        return KeyComp.Compare(x.Key, y.Key);
      }
    }

    public SortedMap() {
      this.tree = new RedBlackTree<KeyValuePair<T1, T2>>(ValueComp);
    }

    public SortedMap(IDictionary<T1, T2> mapA) {
      this.tree = new RedBlackTree<KeyValuePair<T1, T2>>(ValueComp);
      foreach (var item in mapA) {
        this.tree.AddOverwrite(item);
      }
    }

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
      }
      value = default(T2);
      return false;
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
        KeyValuePair<T1, T2> kvpIn;
        kvpIn = new KeyValuePair<T1, T2>(key, default(T2));
        if (
this.tree.Find(
kvpIn,
out kvp)) {
          return kvp.Value;
        }
        throw new KeyNotFoundException("Key not found: " + key);
      }

      set {
        this.tree.AddOverwrite(new KeyValuePair<T1, T2>(key, value));
      }
    }

    public void Add(KeyValuePair<T1, T2> item) {
      this.Add(item.Key, item.Value);
    }

    public void Clear() {
      this.tree.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item) {
      return this.tree.Contains(item);
    }

    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) {
      this.tree.CopyTo(array, arrayIndex);
    }

    public int Count {
      get {
        return this.tree.Count;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.SortedMap`2.IsReadOnly"]/*'/>
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

System.Collections.IEnumerator
      System.Collections.IEnumerable.GetEnumerator() {
      return this.tree.GetEnumerator();
    }
  }
}
