/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace PeterO
{
  internal sealed class ReadOnlyMap<TKey, TValue> : IDictionary<TKey, TValue> {
    private IDictionary<TKey, TValue> wrapped;

    public ReadOnlyMap(IDictionary<TKey, TValue> wrapped) {
      this.wrapped = wrapped;
    }

    public TValue this[TKey key] {
      get {
        return this.wrapped[key];
      }

      set {
        throw new NotSupportedException();
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public ICollection<TKey> Keys {
      get {
        return this.wrapped.Keys;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public ICollection<TValue> Values {
      get {
        return this.wrapped.Values;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public int Count {
      get {
        return this.wrapped.Count;
      }
    }

    /// <summary>Gets a value indicating whether this dictionary is read-only.</summary>
    /// <value>Always true.</value>
    public bool IsReadOnly {
      get {
        return true;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='key'>A TKey object.</param>
    /// <returns>A Boolean object.</returns>
    public bool ContainsKey(TKey key) {
      return this.wrapped.ContainsKey(key);
    }

    /// <summary>Adds two TKey objects.</summary>
    /// <param name='key'>A TKey object.</param>
    /// <param name='value'>A TValue object.</param>
    public void Add(TKey key, TValue value) {
      throw new NotSupportedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='key'>A TKey object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Remove(TKey key) {
      throw new NotSupportedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='key'>A TKey object.</param>
    /// <param name='value'>A TValue object.</param>
    /// <returns>A Boolean object.</returns>
    public bool TryGetValue(TKey key, out TValue value) {
      return this.wrapped.TryGetValue(key, out value);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='item'>A KeyValuePair object.</param>
    public void Add(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    /// <summary>Not documented yet.</summary>
    public void Clear() {
      throw new NotSupportedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='item'>A KeyValuePair object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return this.wrapped.Contains(item);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='array'>A KeyValuePair[] object.</param>
    /// <param name='arrayIndex'>A 32-bit signed integer.</param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      this.wrapped.CopyTo(array, arrayIndex);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='item'>A KeyValuePair object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Remove(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return this.wrapped.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.wrapped.GetEnumerator();
    }
  }
}
