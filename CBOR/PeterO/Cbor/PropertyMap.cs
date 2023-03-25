/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal static class PropertyMap {
    private const int TicksDivFracSeconds =
      CBORUtilities.FractionalSeconds / 10000000;

    private sealed class OrderedDictionary<TKey, TValue> :
      IDictionary<TKey, TValue> {
      // NOTE: Note that this class will be used with CBORObjects, some of which are
      // mutable. Storing mutable keys in an ordinary Dictionary can cause problems;
      // for example:
      // - 'Add'ing a key, then changing it, then calling CompareTo on the changed
      // key can fail to find the key in the Dictionary, even though the old and new
      // versions
      // of the key have the same reference.
      // - The same can happen if an object that contains a Dictionary is 'Add'ed to
      // that
      // Dictionary.
      private readonly IDictionary<TKey, TValue> dict;
      private readonly LinkedList<TKey> list;
      public OrderedDictionary() {
        this.dict = new SortedDictionary<TKey, TValue>();
        this.list = new LinkedList<TKey>();
      }
      public void Add(KeyValuePair<TKey, TValue> kvp) {
        this.Add(kvp.Key, kvp.Value);
      }
      public void Add(TKey k, TValue v) {
        if (this.dict.ContainsKey(k)) {
          throw new ArgumentException("duplicate key");
        } else {
          // CheckKeyDoesNotExist(k);
          // DebugUtility.Log("Adding: " + (k.GetHashCode()) + " [Type=" + (CS(k)) +
          // "]");
          int keycnt = this.dict.Count;
          this.dict.Add(k, v);
          // if (keycnt == this.dict.Count) {
          // throw new InvalidOperationException();
          // }
          this.list.AddLast(k);
          // CheckKeyExists(k);
        }
      }
      public TValue this[TKey key] {
        get {
          TValue v = default(TValue);
          // NOTE: Don't use dict[key], since if it fails it could
          // print the key in the exception's message, which could
          // cause an infinite loop
          if (!this.dict.TryGetValue(key, out v)) {
            throw new ArgumentException("key not found");
          }
          return v;
        }
        set {
          if (this.dict.ContainsKey(key)) {
            // DebugUtility.Log("Set existing: " + (key.GetHashCode()) + " [Type=" +
            // (CS(key)) + "]");
            this.dict[key] = value;
            // CheckKeyExists(key);
          } else {
            // DebugUtility.Log("Set new: " + (key.GetHashCode()) + " [Type=" + (CS(key))
            // +
            // "]");
            this.dict.Add(key, value);
            this.list.AddLast(key);
            // CheckKeyExists(key);
          }
        }
      }
      public void Clear() {
        this.dict.Clear();
        this.list.Clear();
      }
      public void CopyTo(KeyValuePair<TKey, TValue>[] a, int off) {
        foreach (var kv in this) {
          a[off++] = kv;
        }
      }
      public bool Remove(KeyValuePair<TKey, TValue> kvp) {
        if (this.Contains(kvp)) {
          // CheckKeyExists(kvp.Key);
          this.dict.Remove(kvp.Key);
          this.list.Remove(kvp.Key);
          return true;
        }
        return false;
      }
      public bool Remove(TKey key) {
        if (this.dict.Remove(key)) {
          // CheckKeyExists(key);
          this.list.Remove(key);
          return true;
        }
        return false;
      }
      public bool Contains(KeyValuePair<TKey, TValue> kvp) {
        if (this.dict.TryGetValue(kvp.Key, out TValue val)) {
          if (val.Equals(kvp.Value)) {
            return true;
          }
        }
        return false;
      }
      public bool ContainsKey(TKey key) {
        return this.dict.ContainsKey(key);
      }
      public bool TryGetValue(TKey key, out TValue val) {
        return this.dict.TryGetValue(key, out val);
      }
      public int Count {
        get {
          return this.dict.Count;
        }
      }
      public bool IsReadOnly {
        get {
          return false;
        }
      }

      [System.Diagnostics.Conditional("DEBUG")]
      private void CheckKeyExists(TKey key) {
        TValue v = default(TValue);
        if (!this.dict.ContainsKey(key)) {
          /* DebugUtility.Log("hash " + (key.GetHashCode()) + " [" +
          (CS(key)) + "]");
          foreach (var k in this.dict.Keys) {
            DebugUtility.Log(
            "key {0} {1}" + "\u0020" + "\u0020 [{2}]",
                k.Equals(key), k.GetHashCode(), CS(k), CS(key),
              this.dict.ContainsKey(k));
          } */
          throw new ArgumentException("key not found (ContainsKey)");
        }
        // NOTE: Don't use dict[k], since if it fails it could
        // print the key in the exception's message, which could
        // cause an infinite loop
        if (!this.dict.TryGetValue(key, out v)) {
          throw new ArgumentException("key not found (TryGetValue)");
        }
        if (this.dict.Count != this.list.Count) {
          throw new InvalidOperationException();
        }
      }

      [System.Diagnostics.Conditional("DEBUG")]
      private void CheckKeyDoesNotExist(TKey key) {
        TValue v = default(TValue);
        // NOTE: Don't use dict[k], since if it fails it could
        // print the key in the exception's message, which could
        // cause an infinite loop
        if (!this.dict.TryGetValue(key, out v)) {
          return;
        }
        throw new ArgumentException("key found");
      }

      public ICollection<TKey> Keys {
        get {
          return new KeyWrapper<TKey, TValue>(this.dict, this.list);
        }
      }

      public ICollection<TKey> SortedKeys {
        get {
           return this.dict.Keys;
        }
      }

      public ICollection<TValue> Values {
        get {
          return new ValueWrapper<TKey, TValue>(this.dict, this.list);
        }
      }

      private IEnumerable<KeyValuePair<TKey, TValue>> Iterate() {
        foreach (var k in this.list) {
          TValue v = default(TValue);
          // DebugUtility.Log("Enumerating: " + (k.GetHashCode()) + " [Type=" + ((k as
          // CBORObject).Type) + "]");
          // NOTE: Don't use dict[k], since if it fails it could
          // print the key in the exception's message, which could
          // cause an infinite loop
          if (!this.dict.TryGetValue(k, out v)) {
            throw new ArgumentException("key not found");
          }
          yield return new KeyValuePair<TKey, TValue>(k, v);
        }
      }

      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        return this.Iterate().GetEnumerator();
      }
      IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)this.Iterate()).GetEnumerator();
      }
    }

    private sealed class ValueWrapper<TKey, TValue> : ICollection<TValue> {
      private readonly IDictionary<TKey, TValue> dict;
      private readonly LinkedList<TKey> list;
      public ValueWrapper(IDictionary<TKey, TValue> dict, LinkedList<TKey>
        list) {
        this.dict = dict;
        this.list = list;
      }
      public void Add(TValue v) {
        throw new NotSupportedException();
      }
      public void Clear() {
        throw new NotSupportedException();
      }
      public void CopyTo(TValue[] a, int off) {
        foreach (var k in this.list) {
          a[off++] = this.dict[k];
        }
      }
      public bool Remove(TValue v) {
        throw new NotSupportedException();
      }
      public bool Contains(TValue v) {
        foreach (var k in this.list) {
          if (this.dict[k].Equals(v)) {
            return true;
          }
        }
        return false;
      }
      public int Count {
        get {
          return this.dict.Count;
        }
      }
      public bool IsReadOnly {
        get {
          return true;
        }
      }

      private IEnumerable<TValue> Iterate() {
        foreach (var k in this.list) {
          yield return this.dict[k];
        }
      }

      public IEnumerator<TValue> GetEnumerator() {
        return this.Iterate().GetEnumerator();
      }
      IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)this.Iterate()).GetEnumerator();
      }
    }

    private sealed class KeyWrapper<TKey, TValue> : ICollection<TKey> {
      private readonly IDictionary<TKey, TValue> dict;
      private readonly LinkedList<TKey> list;
      public KeyWrapper(IDictionary<TKey, TValue> dict, LinkedList<TKey> list) {
        this.dict = dict;
        this.list = list;
      }
      public void Add(TKey v) {
        throw new NotSupportedException();
      }
      public void Clear() {
        throw new NotSupportedException();
      }
      public void CopyTo(TKey[] a, int off) {
        this.list.CopyTo(a, off);
      }
      public bool Remove(TKey v) {
        throw new NotSupportedException();
      }
      public bool Contains(TKey v) {
        return this.dict.ContainsKey(v);
      }
      public int Count {
        get {
          return this.dict.Count;
        }
      }
      public bool IsReadOnly {
        get {
          return true;
        }
      }
      public IEnumerator<TKey> GetEnumerator() {
        return this.list.GetEnumerator();
      }
      IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)this.list).GetEnumerator();
      }
    }

    private sealed class ReadOnlyWrapper<T> : ICollection<T> {
      private readonly ICollection<T> o;
      public ReadOnlyWrapper(ICollection<T> o) {
        this.o = o;
      }
      public void Add(T v) {
        throw new NotSupportedException();
      }
      public void Clear() {
        throw new NotSupportedException();
      }
      public void CopyTo(T[] a, int off) {
        this.o.CopyTo(a, off);
      }
      public bool Remove(T v) {
        throw new NotSupportedException();
      }
      public bool Contains(T v) {
        return this.o.Contains(v);
      }
      public int Count {
        get {
          return this.o.Count;
        }
      }
      public bool IsReadOnly {
        get {
          return true;
        }
      }
      public IEnumerator<T> GetEnumerator() {
        return this.o.GetEnumerator();
      }
      IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)this.o).GetEnumerator();
      }
    }

    private sealed class PropertyData {
      private readonly string name;
      private readonly MemberInfo prop;
      private readonly string adjustedName;
      private readonly string adjustedNameCamelCase;
      public string Name {
        get {
          return this.name;
        }
      }

      public PropertyData(string name, MemberInfo prop) {
        this.name = name;
        this.prop = prop;
        this.adjustedNameCamelCase = this.GetAdjustedNameInternal(true);
        this.adjustedName = this.GetAdjustedNameInternal(false);
      }

      public Type PropertyType {
        get {
          var pr = this.prop as PropertyInfo;
          if (pr != null) {
            return pr.PropertyType;
          }
          var fi = this.prop as FieldInfo;
          return (fi != null) ? fi.FieldType : null;
        }
      }

      public object GetValue(object obj) {
        var pr = this.prop as PropertyInfo;
        if (pr != null) {
          return pr.GetValue(obj, null);
        }
        var fi = this.prop as FieldInfo;
        return (fi != null) ? fi.GetValue(obj) : null;
      }

      public void SetValue(object obj, object value) {
        var pr = this.prop as PropertyInfo;
        if (pr != null) {
          pr.SetValue(obj, value, null);
        }
        var fi = this.prop as FieldInfo;
        if (fi != null) {
          fi.SetValue(obj, value);
        }
      }

      #if NET20 || NET40
      public static bool HasUsableGetter(PropertyInfo pi) {
        return pi != null && pi.CanRead && !pi.GetGetMethod().IsStatic &&
          pi.GetGetMethod().IsPublic;
      }

      public static bool HasUsableSetter(PropertyInfo pi) {
        return pi != null && pi.CanWrite && !pi.GetSetMethod().IsStatic &&
          pi.GetSetMethod().IsPublic;
      }
      #else
      public static bool HasUsableGetter(PropertyInfo pi) {
        return pi != null && pi.CanRead && !pi.GetMethod.IsStatic &&
          pi.GetMethod.IsPublic;
      }

      public static bool HasUsableSetter(PropertyInfo pi) {
        return pi != null && pi.CanWrite && !pi.SetMethod.IsStatic &&
          pi.SetMethod.IsPublic;
      }
      #endif
      public bool HasUsableGetter() {
        var pr = this.prop as PropertyInfo;
        if (pr != null) {
          return HasUsableGetter(pr);
        }
        var fi = this.prop as FieldInfo;
        return fi != null && fi.IsPublic && !fi.IsStatic &&
          !fi.IsInitOnly && !fi.IsLiteral;
      }

      public bool HasUsableSetter() {
        var pr = this.prop as PropertyInfo;
        if (pr != null) {
          return HasUsableSetter(pr);
        }
        var fi = this.prop as FieldInfo;
        return fi != null && fi.IsPublic && !fi.IsStatic &&
          !fi.IsInitOnly && !fi.IsLiteral;
      }

      public string GetAdjustedName(bool useCamelCase) {
        return useCamelCase ? this.adjustedNameCamelCase :
          this.adjustedName;
      }

      public string GetAdjustedNameInternal(bool useCamelCase) {
        string thisName = this.Name;
        if (useCamelCase) {
          if (CBORUtilities.NameStartsWithWord(thisName, "Is")) {
            thisName = thisName.Substring(2);
          }
          thisName = CBORUtilities.FirstCharLower(thisName);
        } else {
          thisName = CBORUtilities.FirstCharUpper(thisName);
        }
        return thisName;
      }

      public MemberInfo Prop {
        get {
          return this.prop;
        }
      }
    }

    #if NET40 || NET20
    private static bool IsGenericType(Type type) {
      return type.IsGenericType;
    }

    private static bool IsClassOrValueType(Type type) {
      return type.IsClass || type.IsValueType;
    }

    private static Type FirstGenericArgument(Type type) {
      return type.GetGenericArguments()[0];
    }

    private static IEnumerable<PropertyInfo> GetTypeProperties(Type t) {
      return t.GetProperties(BindingFlags.Public |
          BindingFlags.Instance);
    }

    private static IEnumerable<FieldInfo> GetTypeFields(Type t) {
      return t.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    private static IEnumerable<Type> GetTypeInterfaces(Type t) {
      return t.GetInterfaces();
    }

    private static bool IsAssignableFrom(Type superType, Type subType) {
      return superType.IsAssignableFrom(subType);
    }

    private static MethodInfo GetTypeMethod(
      Type t,
      string name,
      Type[] parameters) {
      return t.GetMethod(name, parameters);
    }

    private static bool HasCustomAttribute(
      Type t,
      string name) {
      foreach (var attr in t.GetCustomAttributes(false)) {
        if (attr.GetType().FullName.Equals(name,
            StringComparison.Ordinal)) {
          return true;
        }
      }
      return false;
    }
    #else
    private static bool IsGenericType(Type type) {
      return type.GetTypeInfo().IsGenericType;
    }

    private static bool IsClassOrValueType(Type type) {
      return type.GetTypeInfo().IsClass || type.GetTypeInfo().IsValueType;
    }

    private static Type FirstGenericArgument(Type type) {
      return type.GenericTypeArguments[0];
    }

    private static bool IsAssignableFrom(Type superType, Type subType) {
      return superType.GetTypeInfo().IsAssignableFrom(subType.GetTypeInfo());
    }

    private static IEnumerable<PropertyInfo> GetTypeProperties(Type t) {
      return t.GetRuntimeProperties();
    }

    private static IEnumerable<FieldInfo> GetTypeFields(Type t) {
      return t.GetRuntimeFields();
    }

    private static IEnumerable<Type> GetTypeInterfaces(Type t) {
      return t.GetTypeInfo().ImplementedInterfaces;
    }

    private static MethodInfo GetTypeMethod(
      Type t,
      string name,
      Type[] parameters) {
      return t.GetRuntimeMethod(name, parameters);
    }

    private static bool HasCustomAttribute(
      Type t,
      string name) {
      foreach (var attr in t.GetTypeInfo().GetCustomAttributes()) {
        if (attr.GetType().FullName.Equals(name, StringComparison.Ordinal)) {
          return true;
        }
      }
      return false;
    }
    #endif

    // Give each thread its own version of propertyLists using ThreadStatic
    [ThreadStatic]
    private static IDictionary<Type, IList<PropertyData>>
    propertyLists;

    private static string RemoveIsPrefix(string pn) {
      return CBORUtilities.NameStartsWithWord(pn, "Is") ? pn.Substring(2) :
        pn;
    }

    private static IList<PropertyData> GetPropertyList(Type t) {
      {
        propertyLists = propertyLists ?? new Dictionary<Type, IList<PropertyData>>();
        if (propertyLists.TryGetValue(t, out IList<PropertyData> ret)) {
          return ret;
        }
        ret = new List<PropertyData>();
        bool anonymous = HasCustomAttribute(
            t,
            "System.Runtime.CompilerServices.CompilerGeneratedAttribute") ||
          HasCustomAttribute(
            t,
            "Microsoft.FSharp.Core.CompilationMappingAttribute");
        var names = new SortedDictionary<string, int>();
        foreach (PropertyInfo pi in GetTypeProperties(t)) {
          var pn = RemoveIsPrefix(pi.Name);
          if (names.TryGetValue(pn, out int count)) {
            names[pn] = count + 1;
          } else {
            names[pn] = 1;
          }
        }
        foreach (FieldInfo pi in GetTypeFields(t)) {
          var pn = RemoveIsPrefix(pi.Name);
          if (names.TryGetValue(pn, out int count)) {
            names[pn] = count + 1;
          } else {
            names[pn] = 1;
          }
        }
        foreach (FieldInfo fi in GetTypeFields(t)) {
          PropertyData pd = new PropertyMap.PropertyData(fi.Name, fi);
          if (pd.HasUsableGetter() || pd.HasUsableSetter()) {
            var pn = RemoveIsPrefix(pd.Name);
            // Ignore ambiguous properties
            if (names.TryGetValue(pn, out int count) && count > 1) {
              continue;
            }
            ret.Add(pd);
          }
        }
        foreach (PropertyInfo pi in GetTypeProperties(t)) {
          if (pi.CanRead && (pi.CanWrite || anonymous) &&
            pi.GetIndexParameters().Length == 0) {
            if (PropertyData.HasUsableGetter(pi) ||
              PropertyData.HasUsableSetter(pi)) {
              var pn = RemoveIsPrefix(pi.Name);
              // Ignore ambiguous properties
              if (names.TryGetValue(pn, out int count) && count > 1) {
                continue;
              }
              PropertyData pd = new PropertyMap.PropertyData(pi.Name, pi);
              ret.Add(pd);
            }
          }
        }
        propertyLists.Add(
          t,
          ret);
        return ret;
      }
    }

    public static IList<CBORObject> ListFromArray(CBORObject[] array) {
      return new List<CBORObject>(array);
    }

    public static bool ExceedsKnownLength(Stream inStream, long size) {
      return (inStream is MemoryStream) && (size > (inStream.Length -
            inStream.Position));
    }

    public static void SkipStreamToEnd(Stream inStream) {
      if (inStream is MemoryStream) {
        inStream.Position = inStream.Length;
      }
    }

    public static bool FirstElement(int[] dimensions) {
      foreach (var d in dimensions) {
        if (d == 0) {
          return false;
        }
      }
      return true;
    }

    public static bool NextElement(int[] index, int[] dimensions) {
      for (var i = dimensions.Length - 1; i >= 0; --i) {
        if (dimensions[i] > 0) {
          ++index[i];
          if (index[i] >= dimensions[i]) {
            index[i] = 0;
          } else {
            return true;
          }
        }
      }
      return false;
    }

    public static CBORObject BuildCBORArray(int[] dimensions) {
      int zeroPos = dimensions.Length;
      for (var i = 0; i < dimensions.Length; ++i) {
        if (dimensions[i] == 0) {
          {
            zeroPos = i;
          }
          break;
        }
      }
      int arraydims = zeroPos - 1;
      if (arraydims <= 0) {
        return CBORObject.NewArray();
      }
      var stack = new CBORObject[zeroPos];
      var index = new int[zeroPos];
      var stackpos = 0;
      CBORObject ret = CBORObject.NewArray();
      stack[0] = ret;
      index[0] = 0;
      for (var i = 0; i < dimensions[0]; ++i) {
        ret.Add(CBORObject.NewArray());
      }
      ++stackpos;
      while (stackpos > 0) {
        int curindex = index[stackpos - 1];
        if (curindex < stack[stackpos - 1].Count) {
          CBORObject subobj = stack[stackpos - 1][curindex];
          if (stackpos < zeroPos) {
            stack[stackpos] = subobj;
            index[stackpos] = 0;
            for (var i = 0; i < dimensions[stackpos]; ++i) {
              subobj.Add(CBORObject.NewArray());
            }
            ++index[stackpos - 1];
            ++stackpos;
          } else {
            ++index[stackpos - 1];
          }
        } else {
          --stackpos;
        }
      }
      return ret;
    }

    public static CBORObject FromArray(
      Object arrObj,
      PODOptions options,
      CBORTypeMapper mapper,
      int depth) {
      var arr = (Array)arrObj;
      int rank = arr.Rank;
      if (rank == 0) {
        return CBORObject.NewArray();
      }
      CBORObject obj = null;
      if (rank == 1) {
        // Most common case: the array is one-dimensional
        obj = CBORObject.NewArray();
        int len = arr.GetLength(0);
        for (var i = 0; i < len; ++i) {
          obj.Add(
            CBORObject.FromObject(
              arr.GetValue(i),
              options,
              mapper,
              depth + 1));
        }
        return obj;
      }
      var index = new int[rank];
      var dimensions = new int[rank];
      for (var i = 0; i < rank; ++i) {
        dimensions[i] = arr.GetLength(i);
      }
      if (!FirstElement(dimensions)) {
        return obj;
      }
      obj = BuildCBORArray(dimensions);
      do {
        CBORObject o = CBORObject.FromObject(
            arr.GetValue(index),
            options,
            mapper,
            depth + 1);
        SetCBORObject(obj, index, o);
      } while (NextElement(index, dimensions));
      return obj;
    }

    private static CBORObject GetCBORObject(CBORObject cbor, int[] index) {
      CBORObject ret = cbor;
      foreach (var i in index) {
        ret = ret[i];
      }
      return ret;
    }

    private static void SetCBORObject(
      CBORObject cbor,
      int[] index,
      CBORObject obj) {
      CBORObject ret = cbor;
      for (var i = 0; i < index.Length - 1; ++i) {
        ret = ret[index[i]];
      }
      int ilen = index[index.Length - 1];
      while (ilen >= ret.Count) {
        {
          ret.Add(CBORObject.Null);
        }
      }
      ret[ilen] = obj;
    }

    public static Array FillArray(
      Array arr,
      Type elementType,
      CBORObject cbor,
      CBORTypeMapper mapper,
      PODOptions options,
      int depth) {
      int rank = arr.Rank;
      if (rank == 0) {
        return arr;
      }
      if (rank == 1) {
        int len = arr.GetLength(0);
        for (var i = 0; i < len; ++i) {
          object item = cbor[i].ToObject(
              elementType,
              mapper,
              options,
              depth + 1);
          arr.SetValue(
            item,
            i);
        }
        return arr;
      }
      var index = new int[rank];
      var dimensions = new int[rank];
      for (var i = 0; i < rank; ++i) {
        dimensions[i] = arr.GetLength(i);
      }
      if (!FirstElement(dimensions)) {
        return arr;
      }
      do {
        object item = GetCBORObject(
            cbor,
            index).ToObject(
            elementType,
            mapper,
            options,
            depth + 1);
        arr.SetValue(
          item,
          index);
      } while (NextElement(index, dimensions));
      return arr;
    }

    public static int[] GetDimensions(CBORObject obj) {
      if (obj.Type != CBORType.Array) {
        throw new CBORException();
      }
      // Common cases
      if (obj.Count == 0) {
        return new int[] { 0 };
      }
      if (obj[0].Type != CBORType.Array) {
        return new int[] { obj.Count };
      }
      // Complex cases
      var list = new List<int>();
      list.Add(obj.Count);
      while (obj.Type == CBORType.Array &&
        obj.Count > 0 && obj[0].Type == CBORType.Array) {
        list.Add(obj[0].Count);
        obj = obj[0];
      }
      return list.ToArray();
    }

    public static object ObjectToEnum(CBORObject obj, Type enumType) {
      Type utype = Enum.GetUnderlyingType(enumType);
      object ret = null;
      if (obj.IsNumber && obj.AsNumber().IsInteger()) {
        ret = Enum.ToObject(enumType, TypeToIntegerObject(obj, utype));
        if (!Enum.IsDefined(enumType, ret)) {
          string estr = ret.ToString();
          if (estr == null || estr.Length == 0 || estr[0] == '-' ||
            (estr[0] >= '0' && estr[0] <= '9')) {
            throw new CBORException("Unrecognized enum value: " +
              obj.ToString());
          }
        }
        return ret;
      } else if (obj.Type == CBORType.TextString) {
        var nameString = obj.AsString();
        foreach (var name in Enum.GetNames(enumType)) {
          if (nameString.Equals(name, StringComparison.Ordinal)) {
            return Enum.Parse(enumType, name);
          }
        }
        throw new CBORException("Not found: " + obj.ToString());
      } else {
        throw new CBORException("Unrecognized enum value: " +
          obj.ToString());
      }
    }

    public static object EnumToObject(Enum value) {
      return value.ToString();
    }

    public static object EnumToObjectAsInteger(Enum value) {
      Type t = Enum.GetUnderlyingType(value.GetType());
      if (t.Equals(typeof(ulong))) {
        ulong uvalue = Convert.ToUInt64(value,
            CultureInfo.InvariantCulture);
        return EInteger.FromUInt64(uvalue);
      }
      return t.Equals(typeof(long)) ? Convert.ToInt64(value,
          CultureInfo.InvariantCulture) : (t.Equals(typeof(uint)) ?
          Convert.ToInt64(value,
            CultureInfo.InvariantCulture) :
          Convert.ToInt32(value, CultureInfo.InvariantCulture));
    }

    public static ICollection<TKey>
    GetSortedKeys<TKey, TValue>(
      IDictionary<TKey, TValue> dict) {
      var odict = dict as OrderedDictionary<TKey, TValue>;
      if (odict != null) {
        return odict.SortedKeys;
      }
      var sdict = dict as SortedDictionary<TKey, TValue>;
      if (sdict != null) {
        return sdict.Keys;
      }
      throw new InvalidOperationException("Internal error: Map doesn't" +
"\u0020support sorted keys");
    }

    public static ICollection<KeyValuePair<TKey, TValue>>
    GetEntries<TKey, TValue>(
      IDictionary<TKey, TValue> dict) {
      var c = (ICollection<KeyValuePair<TKey, TValue>>)dict;
      return new ReadOnlyWrapper<KeyValuePair<TKey, TValue>>(c);
    }

    public static IDictionary<CBORObject, CBORObject> NewOrderedDict() {
      return new OrderedDictionary<CBORObject, CBORObject>();
    }

    public static object FindOneArgumentMethod(
      object obj,
      string name,
      Type argtype) {
      return GetTypeMethod(obj.GetType(), name, new[] { argtype });
    }

    public static object InvokeOneArgumentMethod(
      object methodInfo,
      object obj,
      object argument) {
      var mi = (MethodInfo)methodInfo;
      return mi.Invoke(obj, new[] { argument });
    }

    public static byte[] UUIDToBytes(Guid guid) {
      var bytes2 = new byte[16];
      var bytes = guid.ToByteArray();
      Array.Copy(bytes, bytes2, 16);
      // Swap the bytes to conform with the UUID RFC
      bytes2[0] = bytes[3];
      bytes2[1] = bytes[2];
      bytes2[2] = bytes[1];
      bytes2[3] = bytes[0];
      bytes2[4] = bytes[5];
      bytes2[5] = bytes[4];
      bytes2[6] = bytes[7];
      bytes2[7] = bytes[6];
      return bytes2;
    }

    private static bool StartsWith(string str, string pfx) {
      return str != null && str.Length >= pfx.Length &&
        str.Substring(0, pfx.Length).Equals(pfx, StringComparison.Ordinal);
    }

    // TODO: Replace* Legacy with AsNumber methods
    // in next major version
    private static object TypeToIntegerObject(CBORObject objThis, Type t) {
      if (t.Equals(typeof(int))) {
        return objThis.AsInt32();
      }
      if (t.Equals(typeof(short))) {
        return objThis.AsNumber().ToInt16Checked();
      }
      if (t.Equals(typeof(ushort))) {
        return objThis.AsUInt16Legacy();
      }
      if (t.Equals(typeof(byte))) {
        return objThis.AsByteLegacy();
      }
      if (t.Equals(typeof(sbyte))) {
        return objThis.AsSByteLegacy();
      }
      if (t.Equals(typeof(long))) {
        return objThis.AsNumber().ToInt64Checked();
      }
      if (t.Equals(typeof(uint))) {
        return objThis.AsUInt32Legacy();
      }
      if (t.Equals(typeof(ulong))) {
        return objThis.AsUInt64Legacy();
      }
      throw new CBORException("Type not supported");
    }

    public static object TypeToObject(
      CBORObject objThis,
      Type t,
      CBORTypeMapper mapper,
      PODOptions options,
      int depth) {
      if (t.Equals(typeof(int))) {
        return objThis.AsInt32();
      }
      if (t.Equals(typeof(short))) {
        return objThis.AsNumber().ToInt16Checked();
      }
      if (t.Equals(typeof(ushort))) {
        return objThis.AsUInt16Legacy();
      }
      if (t.Equals(typeof(byte))) {
        return objThis.AsByteLegacy();
      }
      if (t.Equals(typeof(sbyte))) {
        return objThis.AsSByteLegacy();
      }
      if (t.Equals(typeof(long))) {
        return objThis.AsNumber().ToInt64Checked();
      }
      if (t.Equals(typeof(uint))) {
        return objThis.AsUInt32Legacy();
      }
      if (t.Equals(typeof(ulong))) {
        return objThis.AsUInt64Legacy();
      }
      if (t.Equals(typeof(double))) {
        return objThis.AsDouble();
      }
      if (t.Equals(typeof(decimal))) {
        return objThis.AsDecimalLegacy();
      }
      if (t.Equals(typeof(float))) {
        return objThis.AsSingle();
      }
      if (t.Equals(typeof(bool))) {
        return objThis.AsBoolean();
      }
      if (t.Equals(typeof(char))) {
        if (objThis.Type == CBORType.TextString) {
          string s = objThis.AsString();
          if (s.Length != 1) {
            throw new CBORException("Can't convert to char");
          }
          return s[0];
        }
        if (objThis.IsNumber && objThis.AsNumber().CanFitInInt32()) {
          int c = objThis.AsNumber().ToInt32IfExact();
          if (c < 0 || c >= 0x10000) {
            throw new CBORException("Can't convert to char");
          }
          return (char)c;
        }
        throw new CBORException("Can't convert to char");
      }
      if (t.Equals(typeof(DateTime))) {
        return new CBORDateConverter().FromCBORObject(objThis);
      }
      if (t.Equals(typeof(Guid))) {
        return new CBORUuidConverter().FromCBORObject(objThis);
      }
      if (t.Equals(typeof(Uri))) {
        return new CBORUriConverter().FromCBORObject(objThis);
      }
      if (IsAssignableFrom(typeof(Enum), t)) {
        return ObjectToEnum(objThis, t);
      }
      if (IsGenericType(t)) {
        Type td = t.GetGenericTypeDefinition();
        // Nullable types
        if (td.Equals(typeof(Nullable<>))) {
          Type nullableType = Nullable.GetUnderlyingType(t);
          if (objThis.IsNull) {
            return Activator.CreateInstance(t);
          } else {
            object wrappedObj = objThis.ToObject(
                nullableType,
                mapper,
                options,
                depth + 1);
            return Activator.CreateInstance(
                t,
                wrappedObj);
          }
        }
      }
      if (objThis.Type == CBORType.ByteString) {
        // TODO: Consider converting base64 strings
        if (t.Equals(typeof(byte[]))) {
          byte[] bytes = objThis.GetByteString();
          var byteret = new byte[bytes.Length];
          Array.Copy(bytes, 0, byteret, 0, byteret.Length);
          return byteret;
        }
      }
      if (objThis.Type == CBORType.Array) {
        Type objectType = typeof(object);
        var isList = false;
        var isReadOnlyCollection = false;
        object listObject = null;
        object genericListObject = null;
        if (IsAssignableFrom(typeof(Array), t)) {
          Type elementType = t.GetElementType();
          Array array = Array.CreateInstance(
              elementType,
              GetDimensions(objThis));
          return FillArray(
              array,
              elementType,
              objThis,
              mapper,
              options,
              depth);
        }
        #if NET40 || NET20
        if (t.IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isList = td.Equals(typeof(List<>)) || td.Equals(typeof(IList<>)) ||
            td.Equals(typeof(ICollection<>)) ||
            td.Equals(typeof(IEnumerable<>));
          #if NET20 || NET40
          isReadOnlyCollection = false;
          #else
          isReadOnlyCollection = (td.Equals(typeof(IReadOnlyCollection<>)) ||
             td.Equals(typeof(IReadOnlyList<>)) ||

             td.Equals(
               typeof(System.Collections.ObjectModel.ReadOnlyCollection<>)))
&&
             t.GetGenericArguments().Length == 1;
         #endif
        }
        isList = isList && t.GetGenericArguments().Length == 1;
        if (isReadOnlyCollection) {
          objectType = t.GetGenericArguments()[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        } else if (isList) {
          objectType = t.GetGenericArguments()[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        }
        #else
        // TODO: Support IReadOnlyDictionary
        if (t.GetTypeInfo().IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isList = td.Equals(typeof(List<>)) || td.Equals(typeof(IList<>)) ||
            td.Equals(typeof(ICollection<>)) ||
            td.Equals(typeof(IEnumerable<>));
          isReadOnlyCollection = (td.Equals(typeof(IReadOnlyCollection<>)) ||
             td.Equals(typeof(IReadOnlyList<>)) ||

             td.Equals(
               typeof(System.Collections.ObjectModel.ReadOnlyCollection<>)))
&&
             t.GenericTypeArguments.Length == 1;
        }
        isList = isList && t.GenericTypeArguments.Length == 1;
        if (isReadOnlyCollection) {
          objectType = t.GenericTypeArguments[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        } else if (isList) {
          objectType = t.GenericTypeArguments[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        }
        #endif
        if (listObject == null) {
          if (t.Equals(typeof(IList)) ||
            t.Equals(typeof(ICollection)) || t.Equals(typeof(IEnumerable))) {
            listObject = new List<object>();
            objectType = typeof(object);
          } else if (IsClassOrValueType(t)) {
            var implementsList = false;
            foreach (var interf in GetTypeInterfaces(t)) {
              if (IsGenericType(interf) &&
                interf.GetGenericTypeDefinition().Equals(typeof(IList<>))) {
                if (implementsList) {
                  implementsList = false;
                  break;
                } else {
                  implementsList = true;
                  objectType = FirstGenericArgument(interf);
                }
              }
            }
            if (implementsList) {
              // DebugUtility.Log("assignable from ilist<>");
              genericListObject = Activator.CreateInstance(t);
            } else {
              // DebugUtility.Log("not assignable from ilist<> " + t);
            }
          }
        }
        if (genericListObject != null) {
          object addMethod = FindOneArgumentMethod(
              genericListObject,
              "Add",
              objectType);
          if (addMethod == null) {
            throw new CBORException("no add method");
          }
          foreach (CBORObject value in objThis.Values) {
            PropertyMap.InvokeOneArgumentMethod(
              addMethod,
              genericListObject,
              value.ToObject(objectType, mapper, options, depth + 1));
          }
          return genericListObject;
        }
        if (listObject != null) {
          System.Collections.IList ie = (System.Collections.IList)listObject;
          foreach (CBORObject value in objThis.Values) {
            ie.Add(value.ToObject(objectType, mapper, options, depth + 1));
          }
          if (isReadOnlyCollection) {
            objectType = FirstGenericArgument(t);
            Type rocType =
typeof(System.Collections.ObjectModel.ReadOnlyCollection<>)
               .MakeGenericType(objectType);
            listObject = Activator.CreateInstance(rocType, listObject);
          }
          return listObject;
        }
      }
      if (objThis.Type == CBORType.Map) {
        var isDict = false;
        var isReadOnlyDict = false;
        Type keyType = null;
        Type valueType = null;
        object dictObject = null;
        #if NET40 || NET20
        isDict = t.IsGenericType;
        if (t.IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isDict = td.Equals(typeof(Dictionary<,>)) ||
            td.Equals(typeof(IDictionary<,>));
          #if NET20 || NET40
          isReadOnlyDict = false;
          #else
          isReadOnlyDict =
(td.Equals(typeof(System.Collections.ObjectModel.ReadOnlyDictionary<,>)) ||
            td.Equals(typeof(IReadOnlyDictionary<,>))) &&
            t.GetGenericArguments().Length == 2;
          #endif
        }
        // DebugUtility.Log("list=" + isDict);
        isDict = isDict && t.GetGenericArguments().Length == 2;
        // DebugUtility.Log("list=" + isDict);
        if (isDict || isReadOnlyDict) {
          keyType = t.GetGenericArguments()[0];
          valueType = t.GetGenericArguments()[1];
          Type listType = typeof(Dictionary<,>).MakeGenericType(
              keyType,
              valueType);
          dictObject = Activator.CreateInstance(listType);
        }
        #else
        isDict = t.GetTypeInfo().IsGenericType;
        if (t.GetTypeInfo().IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isDict = td.Equals(typeof(Dictionary<,>)) ||
            td.Equals(typeof(IDictionary<,>));
          isReadOnlyDict =
(td.Equals(typeof(System.Collections.ObjectModel.ReadOnlyDictionary<,>)) ||
            td.Equals(typeof(IReadOnlyDictionary<,>))) &&
            t.GenericTypeArguments.Length == 2;
        }
        // DebugUtility.Log("list=" + isDict);
        isDict = isDict && t.GenericTypeArguments.Length == 2;
        // DebugUtility.Log("list=" + isDict);
        if (isDict || isReadOnlyDict) {
          keyType = t.GenericTypeArguments[0];
          valueType = t.GenericTypeArguments[1];
          Type listType = typeof(Dictionary<,>).MakeGenericType(
              keyType,
              valueType);
          dictObject = Activator.CreateInstance(listType);
        }
        #endif
        if (dictObject == null) {
          if (t.Equals(typeof(IDictionary))) {
            dictObject = new Dictionary<object, object>();
            keyType = typeof(object);
            valueType = typeof(object);
          }
        }
        if (dictObject != null) {
          System.Collections.IDictionary idic =
            (System.Collections.IDictionary)dictObject;
          foreach (CBORObject key in objThis.Keys) {
            CBORObject value = objThis[key];
            idic.Add(
              key.ToObject(keyType, mapper, options, depth + 1),
              value.ToObject(valueType, mapper, options, depth + 1));
          }
          #if !NET20 && !NET40
          if (isReadOnlyDict) {
            Type listType =
typeof(
  System.Collections.ObjectModel.ReadOnlyDictionary<,>)
           .MakeGenericType(keyType,
              valueType);
            dictObject = Activator.CreateInstance(listType, dictObject);
          }
          #endif
          return dictObject;
        }
        if (mapper != null) {
          if (!mapper.FilterTypeName(t.FullName)) {
            throw new CBORException("Type " + t.FullName +
              " not supported");
          }
        } else {
          if (t.FullName != null && (
              StartsWith(t.FullName, "Microsoft.Win32.") ||
              StartsWith(t.FullName, "System.IO."))) {
            throw new CBORException("Type " + t.FullName +
              " not supported");
          }
          if (StartsWith(t.FullName, "System.") &&
            !HasCustomAttribute(t, "System.SerializableAttribute")) {
            throw new CBORException("Type " + t.FullName +
              " not supported");
          }
        }
        var values = new List<KeyValuePair<string, CBORObject>>();
        var propNames = PropertyMap.GetPropertyNames(
            t,
            options == null || options.UseCamelCase);
        foreach (string key in propNames) {
          if (objThis.ContainsKey(key)) {
            CBORObject cborValue = objThis[key];
            var dict = new KeyValuePair<string, CBORObject>(
              key,
              cborValue);
            values.Add(dict);
          }
        }
        return PropertyMap.ObjectWithProperties(
            t,
            values,
            mapper,
            options,
            depth);
      } else {
        throw new CBORException();
      }
    }

    public static CBORObject GetOrDefault(IDictionary<CBORObject, CBORObject> map,
      CBORObject key,
      CBORObject defaultValue) {
      CBORObject ret = null;
      return (!map.TryGetValue(key, out ret)) ? defaultValue : ret;
    }

#pragma warning disable CA1801
    public static CBORObject FromObjectOther(object obj) {
      return null;
    }
#pragma warning restore CA1801

    public static object ObjectWithProperties(
      Type t,
      IEnumerable<KeyValuePair<string, CBORObject>> keysValues,
      CBORTypeMapper mapper,
      PODOptions options,
      int depth) {
      try {
        object o = Activator.CreateInstance(t);
        var dict = new SortedDictionary<string, CBORObject>();
        foreach (var kv in keysValues) {
          var name = kv.Key;
          dict[name] = kv.Value;
        }
        foreach (PropertyData key in GetPropertyList(o.GetType())) {
          if (!key.HasUsableSetter() || !key.HasUsableGetter()) {
            // Require properties to have both a setter and
            // a getter to be eligible for setting
            continue;
          }
          var name = key.GetAdjustedName(options != null ?
              options.UseCamelCase : true);
          if (dict.TryGetValue(name, out CBORObject cborObj)) {
            object dobj = cborObj.ToObject(
                key.PropertyType,
                mapper,
                options,
                depth + 1);
            key.SetValue(
              o,
              dobj);
          }
        }
        return o;
      } catch (Exception ex) {
        throw new CBORException(ex.Message, ex);
      }
    }

    public static CBORObject CallToObject(
      CBORTypeMapper.ConverterInfo convinfo,
      object obj) {
      return (CBORObject)PropertyMap.InvokeOneArgumentMethod(
          convinfo.ToObject,
          convinfo.Converter,
          obj);
    }

    public static object CallFromObject(
      CBORTypeMapper.ConverterInfo convinfo,
      CBORObject obj) {
      return (object)PropertyMap.InvokeOneArgumentMethod(
          convinfo.FromObject,
          convinfo.Converter,
          obj);
    }

    public static IEnumerable<KeyValuePair<string, object>> GetProperties(
      Object o) {
      return GetProperties(o, true);
    }

    public static IEnumerable<string> GetPropertyNames(Type t, bool
      useCamelCase) {
      foreach (PropertyData key in GetPropertyList(t)) {
        yield return key.GetAdjustedName(useCamelCase);
      }
    }

    public static IEnumerable<KeyValuePair<string, object>> GetProperties(
      Object o,
      bool useCamelCase) {
      foreach (PropertyData key in GetPropertyList(o.GetType())) {
        if (!key.HasUsableGetter()) {
          continue;
        }
        yield return new KeyValuePair<string, object>(
            key.GetAdjustedName(useCamelCase),
            key.GetValue(o));
      }
    }

    public static void BreakDownDateTime(
      DateTime bi,
      EInteger[] year,
      int[] lf) {
      if (TicksDivFracSeconds == 0) {
        throw new InvalidOperationException();
      }
      #if NET20
      DateTime dt = bi.ToUniversalTime();
      #else
      DateTime dt = TimeZoneInfo.ConvertTime(bi, TimeZoneInfo.Utc);
      #endif
      year[0] = EInteger.FromInt32(dt.Year);
      lf[0] = dt.Month;
      lf[1] = dt.Day;
      lf[2] = dt.Hour;
      lf[3] = dt.Minute;
      lf[4] = dt.Second;
      // lf[5] is the number of nanoseconds
      lf[5] = (int)(dt.Ticks % 10000000L) * TicksDivFracSeconds;
    }

    public static DateTime BuildUpDateTime(EInteger year, int[] dt) {
      if (TicksDivFracSeconds == 0) {
        throw new InvalidOperationException();
      }
      if (year.CompareTo(9999) > 0 || year.CompareTo(0) <= 0) {
        throw new CBORException("Year is too big or too small for DateTime.");
      }
      return new DateTime(
          year.ToInt32Checked(),
          dt[0],
          dt[1],
          dt[2],
          dt[3],
          dt[4],
          DateTimeKind.Utc).AddMinutes(-dt[6]).AddTicks((long)(dt[5] /
            TicksDivFracSeconds));
    }
  }
}
