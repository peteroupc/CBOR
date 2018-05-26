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

      public PropertyInfo Prop {
        get {
          return this.prop;
        }

        set {
          this.prop = value;
        }
      }
    }

#if NET40
    private static IEnumerable<PropertyInfo> GetTypeProperties(Type t) {
      return t.GetProperties(BindingFlags.Public |
        BindingFlags.Instance);
    }
    private static MethodInfo GetTypeMethod(Type t, string name,
      Type[] parameters) {
       return t.GetMethod(name, parameters);
    }
        private static bool HasCustomAttribute (
  Type t,
  string name) {
            foreach (var attr in t.CustomAttributes) {
                DebugUtility.Log (attr.AttributeType.GetType ().FullName);
                if (attr.GetType ().FullName.Equals (name)) {
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
        DebugUtility.Log(attr.GetType().FullName);
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
        IList<PropertyData> ret;
        if (ValuePropertyLists.TryGetValue(t, out ret)) {
          return ret;
        }
        ret = new List<PropertyData>();
        bool anonymous = HasCustomAttribute(
          t,
          "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        foreach (PropertyInfo pi in GetTypeProperties(t)) {
          if (pi.CanRead && (pi.CanWrite || anonymous) &&
          pi.GetIndexParameters().Length == 0) {
            PropertyData pd = new PropertyMap.PropertyData();
            pd.Name = pi.Name;
            // Convert 'IsXYZ' to 'XYZ'
            if (POCOOptions.RemoveIsPrefix && pd.Name.Length >= 3 && pd.Name[0] == 'I' && pd.Name[1] == 's' &&
                pd.Name[2] >= 'A' && pd.Name[2] <= 'Z') {
              pd.Name = pd.Name.Substring(2);
            }
            // Convert to camel case
            if (POCOOptions.UseCamelCase && pd.Name[0] >= 'A' && pd.Name[0] <= 'Z') {
              var sb = new System.Text.StringBuilder();
              sb.Append((char)(pd.Name[0] + 0x20));
              sb.Append(pd.Name.Substring(1));
              pd.Name = sb.ToString();
            }
            pd.Prop = pi;
            ret.Add(pd);
          }
        }
        ValuePropertyLists.Add(t, ret);
        return ret;
      }
    }

    public static bool ExceedsKnownLength(Stream inStream, long size) {
      return (inStream is MemoryStream) ? (size > (inStream.Length -
        inStream.Position)) : false;
    }

    public static void SkipStreamToEnd(Stream inStream) {
      if (inStream is MemoryStream) {
        inStream.Position = inStream.Length;
      }
    }

// Inappropriate to mark these obsolete; they're
// just non-publicly-visible methods to convert to
// and from legacy arbitrary-precision classes
#pragma warning disable 618
    public static BigInteger ToLegacy(EInteger ei) {
      return BigInteger.ToLegacy(ei);
    }

    public static ExtendedDecimal ToLegacy(EDecimal ed) {
      return ExtendedDecimal.ToLegacy(ed);
    }

    public static ExtendedFloat ToLegacy(EFloat ef) {
      return ExtendedFloat.ToLegacy(ef);
    }

    public static ExtendedRational ToLegacy(ERational er) {
      return ExtendedRational.ToLegacy(er);
    }

    public static EInteger FromLegacy(BigInteger ei) {
      return BigInteger.FromLegacy(ei);
    }

    public static EDecimal FromLegacy(ExtendedDecimal ed) {
      return ExtendedDecimal.FromLegacy(ed);
    }

    public static EFloat FromLegacy(ExtendedFloat ef) {
      return ExtendedFloat.FromLegacy(ef);
    }

    public static ERational FromLegacy(ExtendedRational er) {
      return ExtendedRational.FromLegacy(er);
    }
#pragma warning restore 618
    private static void FromArrayRecursive(
  Array arr,
  int[] index,
  int dimension,
  CBORObject obj) {
      int dimLength = arr.GetLength(dimension);
      int rank = index.Length;
      for (var i = 0; i < dimLength; ++i) {
        if (dimension + 1 == rank) {
          index[dimension] = i;
          obj.Add(CBORObject.FromObject(arr.GetValue(index)));
        } else {
          CBORObject child = CBORObject.NewArray();
          for (int j = dimension + 1; j < dimLength; ++j) {
            index[j] = 0;
          }
          FromArrayRecursive(arr, index, dimension + 1, child);
          obj.Add(child);
        }
      }
    }

    public static CBORObject FromArray(Object arrObj) {
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
          obj.Add(CBORObject.FromObject(arr.GetValue(i)));
        }
        return obj;
      }
      var index = new int[rank];
      obj = CBORObject.NewArray();
      FromArrayRecursive(arr, index, 0, obj);
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

    public static IEnumerable<KeyValuePair<string, object>>
    GetProperties(Object o) {
      foreach (PropertyData key in GetPropertyList(o.GetType())) {
        yield return new KeyValuePair<string, object>(
  key.Name,
  key.Prop.GetValue(o, null));
      }
    }
  }
}
