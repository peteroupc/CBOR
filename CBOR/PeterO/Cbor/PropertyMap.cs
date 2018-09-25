/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal static class PropertyMap {
    private sealed class PropertyData {
      private string name;

      public string Name {
        get {
          return this.name;
        }

        set {
          this.name = value;
        }
      }

      private PropertyInfo prop;
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
        return HasUsableGetter(this.prop);
      }

      public bool HasUsableSetter() {
        return HasUsableSetter(this.prop);
      }

      public string GetAdjustedName(bool removeIsPrefix, bool useCamelCase) {
        string thisName = this.Name;
        if(useCamelCase){
          if(CBORUtilities.NameStartsWithWord(thisName,"Is")){
            thisName = thisName.Substring(2);
          }
          thisName = CBORUtilities.FirstCharLower(thisName);
        } else {
          thisName = CBORUtilities.FirstCharUpper(thisName);
        }
        return thisName;
      }

      public PropertyInfo Prop {
        get {
          return this.prop;
        }

        set {
          this.prop = value;
        }
      }
    }

#if NET40 || NET20
    private static IEnumerable<PropertyInfo> GetTypeProperties(Type t) {
      return t.GetProperties(BindingFlags.Public |
        BindingFlags.Instance);
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
#if NET40 || NET20
      foreach (var attr in t.GetCustomAttributes(false)) {
#else
    foreach (var attr in t.CustomAttributes) {
#endif
        if (attr.GetType().FullName.Equals(name)) {
          return true;
        }
      }
      return false;
    }
#else
    private static IEnumerable<PropertyInfo> GetTypeProperties(Type t) {
      return t.GetRuntimeProperties();
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
        if (attr.GetType().FullName.Equals(name)) {
          return true;
        }
      }
      return false;
    }

#endif

    private static readonly IDictionary<Type, IList<PropertyData>>
      ValuePropertyLists = new Dictionary<Type, IList<PropertyData>>();

    private static IList<PropertyData> GetPropertyList(Type t) {
      lock (ValuePropertyLists) {
        if (
 ValuePropertyLists.TryGetValue(
 t,
 out IList<PropertyData> ret)) {
          return ret;
        }
        ret = new List<PropertyData>();
        bool anonymous = HasCustomAttribute(
          t,
          "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        foreach (PropertyInfo pi in GetTypeProperties(t)) {
          if (pi.CanRead && (pi.CanWrite || anonymous) &&
          pi.GetIndexParameters().Length == 0) {
            if (PropertyData.HasUsableGetter(pi) ||
                PropertyData.HasUsableSetter(pi)) {
              PropertyData pd = new PropertyMap.PropertyData() {
                Name = pi.Name,
                Prop = pi
              };
              ret.Add(pd);
            }
          }
        }
        ValuePropertyLists.Add(t, ret);
        return ret;
      }
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

    private static void FromArrayRecursive(
 Array arr,
 int[] index,
 int dimension,
 CBORObject obj,
 PODOptions options,
 CBORTypeMapper mapper,
 int depth) {
      int dimLength = arr.GetLength(dimension);
      int rank = index.Length;
      for (var i = 0; i < dimLength; ++i) {
        if (dimension + 1 == rank) {
          index[dimension] = i;
          obj.Add(
  CBORObject.FromObject(
  arr.GetValue(index),
  options,
  mapper,
  depth + 1));
        } else {
          CBORObject child = CBORObject.NewArray();
          for (int j = dimension + 1; j < dimLength; ++j) {
            index[j] = 0;
          }
        FromArrayRecursive(
  arr,
  index,
  dimension + 1,
  child,
  options,
  mapper,
  depth + 1);
          obj.Add(child);
        }
      }
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
      obj = CBORObject.NewArray();
      FromArrayRecursive(arr, index, 0, obj, options, mapper, depth);
      return obj;
    }

    public static object EnumToObject(Enum value) {
      Type t = Enum.GetUnderlyingType(value.GetType());
      if (t.Equals(typeof(ulong))) {
        var data = new byte[13];
        ulong uvalue = Convert.ToUInt64(value);
        data[0] = (byte)(uvalue & 0xff);
        data[1] = (byte)((uvalue >> 8) & 0xff);
        data[2] = (byte)((uvalue >> 16) & 0xff);
        data[3] = (byte)((uvalue >> 24) & 0xff);
        data[4] = (byte)((uvalue >> 32) & 0xff);
        data[5] = (byte)((uvalue >> 40) & 0xff);
        data[6] = (byte)((uvalue >> 48) & 0xff);
        data[7] = (byte)((uvalue >> 56) & 0xff);
        data[8] = (byte)0;
        return EInteger.FromBytes(data, true);
      }
      return t.Equals(typeof(long)) ? Convert.ToInt64(value) :
      (t.Equals(typeof(uint)) ? Convert.ToInt64(value) :
      Convert.ToInt32(value));
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
      return ((MethodInfo)methodInfo).Invoke(obj, new[] { argument });
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
        str.Substring(0, pfx.Length).Equals(pfx);
    }

    public static object TypeToObject(CBORObject objThis, Type t) {
      if (t.Equals(typeof(int))) {
        return objThis.AsInt32();
      }
      if (t.Equals(typeof(short))) {
        return objThis.AsInt16();
      }
      if (t.Equals(typeof(ushort))) {
        return objThis.AsUInt16();
      }
      if (t.Equals(typeof(byte))) {
        return objThis.AsByte();
      }
      if (t.Equals(typeof(sbyte))) {
        return objThis.AsSByte();
      }
      if (t.Equals(typeof(long))) {
        return objThis.AsInt64();
      }
      if (t.Equals(typeof(uint))) {
        return objThis.AsUInt32();
      }
      if (t.Equals(typeof(ulong))) {
        return objThis.AsUInt64();
      }
      if (t.Equals(typeof(double))) {
        return objThis.AsDouble();
      }
      if (t.Equals(typeof(decimal))) {
        return objThis.AsDecimal();
      }
      if (t.Equals(typeof(float))) {
        return objThis.AsSingle();
      }
      if (t.Equals(typeof(bool))) {
        return objThis.AsBoolean();
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
      if (t.Equals(typeof(EDecimal))) {
        return objThis.AsEDecimal();
      }
      if (t.Equals(typeof(EFloat))) {
        return objThis.AsEFloat();
      }
      if (t.Equals(typeof(EInteger))) {
        return objThis.AsEInteger();
      }
      if (t.Equals(typeof(ERational))) {
        return objThis.AsERational();
      }
      if (objThis.Type == CBORType.ByteString) {
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
        object listObject = null;
#if NET40 || NET20
    if (typeof(Array).IsAssignableFrom(t)) {
Type elementType = t.GetElementType();
          Array array = Array.CreateInstance(
        elementType,
        objThis.Count);
         if (array.Rank != 1) {
 throw new
  NotSupportedException("Multidimensional arrays not supported yet.");
         }
          for (var i = 0; i < objThis.Count; ++i) {
            array.SetValue(objThis[i].ToObject(elementType), i);
          }
    return array;
        }
        if (t.IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isList = td.Equals(typeof(List<>)) || td.Equals(typeof(IList<>)) ||
            td.Equals(typeof(ICollection<>)) ||
            td.Equals(typeof(IEnumerable<>));
            } else {
          throw new NotImplementedException();
        }
        isList = isList && t.GetGenericArguments().Length == 1;
        if (isList) {
          objectType = t.GetGenericArguments()[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        }
#else
if (typeof(Array).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())) {
          Type elementType = t.GetElementType();
          Array array = Array.CreateInstance(
        elementType,
        objThis.Count);
          for (var i = 0; i < objThis.Count; ++i) {
            array.SetValue(objThis[i].ToObject(elementType), i);
          }
    return array;
        }
        if (t.GetTypeInfo().IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isList = (td.Equals(typeof(List<>)) ||
  td.Equals(typeof(IList<>)) ||
  td.Equals(typeof(ICollection<>)) ||
  td.Equals(typeof(IEnumerable<>)));
        } else {
          throw new NotImplementedException();
        }
        isList = (isList && t.GenericTypeArguments.Length == 1);
        if (isList) {
          objectType = t.GenericTypeArguments[0];
          Type listType = typeof(List<>).MakeGenericType(objectType);
          listObject = Activator.CreateInstance(listType);
        }
#endif
        if (listObject != null) {
          System.Collections.IList ie = (System.Collections.IList)listObject;
          foreach (CBORObject value in objThis.Values) {
            ie.Add(value.ToObject(objectType));
          }
          return listObject;
        }
      }
      if (objThis.Type == CBORType.Map) {
        var isDict = false;
        Type keyType = null;
        Type valueType = null;
        object dictObject = null;
#if NET40 || NET20
        isDict = t.IsGenericType;
        if (t.IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isDict = td.Equals(typeof(Dictionary<, >)) ||
            td.Equals(typeof(IDictionary<, >));
        }
        // DebugUtility.Log("list=" + isDict);
        isDict = isDict && t.GetGenericArguments().Length == 2;
        // DebugUtility.Log("list=" + isDict);
        if (isDict) {
          keyType = t.GetGenericArguments()[0];
          valueType = t.GetGenericArguments()[1];
          Type listType = typeof(Dictionary<, >).MakeGenericType(
            keyType,
            valueType);
          dictObject = Activator.CreateInstance(listType);
        }
#else
        isDict = (t.GetTypeInfo().IsGenericType);
        if (t.GetTypeInfo().IsGenericType) {
          Type td = t.GetGenericTypeDefinition();
          isDict = (td.Equals(typeof(Dictionary<, >)) ||
  td.Equals(typeof(IDictionary<, >)));
        }
        //DebugUtility.Log("list=" + isDict);
        isDict = (isDict && t.GenericTypeArguments.Length == 2);
        //DebugUtility.Log("list=" + isDict);
        if (isDict) {
          keyType = t.GenericTypeArguments[0];
          valueType = t.GenericTypeArguments[1];
          Type listType = typeof(Dictionary<, >).MakeGenericType(
            keyType,
            valueType);
          dictObject = Activator.CreateInstance(listType);
        }
#endif
        if (dictObject != null) {
          System.Collections.IDictionary idic =
            (System.Collections.IDictionary)dictObject;
          foreach (CBORObject key in objThis.Keys) {
            CBORObject value = objThis[key];
            idic.Add(
  key.ToObject(keyType),
  value.ToObject(valueType));
          }
          return dictObject;
        }
        if (t.FullName != null && (StartsWith(t.FullName, "Microsoft.Win32.") ||
           StartsWith(t.FullName, "System.IO."))) {
      throw new CBORException("Type " + t.FullName +
            " not supported");
        }
        if (StartsWith(t.FullName, "System.") &&
          !HasCustomAttribute(t, "System.SerializableAttribute")) {
      throw new CBORException("Type " + t.FullName +
            " not supported");
        }
        var values = new List<KeyValuePair<string, CBORObject>>();
        foreach (string key in PropertyMap.GetPropertyNames(
                   t,
                   true,
                   true)) {
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
    true,
    true);
      } else {
        throw new CBORException();
      }
    }

    public static object ObjectWithProperties(
      Type t,
      IEnumerable<KeyValuePair<string, CBORObject>> keysValues) {
      return ObjectWithProperties(t, keysValues, true, true);
    }

    public static object ObjectWithProperties(
         Type t,
         IEnumerable<KeyValuePair<string, CBORObject>> keysValues,
         bool removeIsPrefix,
  bool useCamelCase) {
      object o = Activator.CreateInstance(t);
      var dict = new Dictionary<string, CBORObject>();
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
        var name = key.GetAdjustedName(removeIsPrefix, useCamelCase);
        if (dict.ContainsKey(name)) {
          object dobj = dict[name].ToObject(key.Prop.PropertyType);
          key.Prop.SetValue(o, dobj, null);
        }
      }
      return o;
    }

    public static IEnumerable<KeyValuePair<string, object>>
    GetProperties(Object o) {
      return GetProperties(o, true, true);
    }

    public static IEnumerable<string>
    GetPropertyNames(Type t, bool removeIsPrefix, bool useCamelCase) {
      foreach (PropertyData key in GetPropertyList(t)) {
        yield return key.GetAdjustedName(removeIsPrefix, useCamelCase);
      }
    }

    public static IEnumerable<KeyValuePair<string, object>>
    GetProperties(Object o, bool removeIsPrefix, bool useCamelCase) {
      foreach (PropertyData key in GetPropertyList(o.GetType())) {
        if (!key.HasUsableGetter()) {
          continue;
        }
        yield return new KeyValuePair<string, object>(
  key.GetAdjustedName(removeIsPrefix, useCamelCase),
  key.Prop.GetValue(o, null));
      }
    }

    public static void BreakDownDateTime(
  DateTime bi,
  EInteger[] year,
  int[] lf) {
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
      lf[5] = (int)(dt.Ticks % 10000000L) * 100;
    }

    public static DateTime BuildUpDateTime(EInteger year, int[] dt) {
      return new DateTime(
  year.ToInt32Checked(),
  dt[0],
  dt[1],
  dt[2],
  dt[3],
  dt[4],
  DateTimeKind.Utc).AddMinutes(-dt[6]).AddTicks((long)(dt[5] / 100));
    }
  }
}
