/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using PeterO;

namespace PeterO.Cbor {
  internal class PropertyMap
  {
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

    private static IDictionary<Type, IList<PropertyData>> propertyLists =
      new Dictionary<Type, IList<PropertyData>>();

    private static IList<PropertyData> GetPropertyList(Type t) {
      lock (propertyLists) {
        IList<PropertyData> ret;
        if (propertyLists.TryGetValue(t, out ret)) {
          return ret;
        }
        ret = new List<PropertyData>();
        bool anonymous = t.Name.Contains("__AnonymousType");
        foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
          if (pi.CanRead && (pi.CanWrite || anonymous) && pi.GetIndexParameters().Length == 0) {
            PropertyData pd = new PropertyMap.PropertyData();
            pd.Name = pi.Name;
            // Convert 'IsXYZ' to 'XYZ'
            if (pd.Name.Length >= 3 &&
                pd.Name[0] == 'I' && pd.Name[1] == 's' &&
                pd.Name[2] >= 'A' && pd.Name[2] == 'Z') {
              pd.Name = pd.Name.Substring(2);
            }
            // Convert to camel case
            if (pd.Name[0] >= 'A' && pd.Name[0] <= 'Z') {
              System.Text.StringBuilder sb = new System.Text.StringBuilder();
              sb.Append((char)(pd.Name[0] + 0x20));
              sb.Append(pd.Name.Substring(1));
              pd.Name = sb.ToString();
            }
            pd.Prop = pi;
            ret.Add(pd);
          }
        }
        propertyLists.Add(t, ret);
        return ret;
      }
    }

    private static void FromArrayRecursive(
      Array arr,
      int[] index,
      int dimension,
      CBORObject obj) {
      int dimLength = arr.GetLength(dimension);
      int rank = index.Length;
      for (int i = 0; i < dimLength; ++i) {
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
      Array arr = (Array)arrObj;
      int rank = arr.Rank;
      if (rank == 0) {
        return CBORObject.NewArray();
      }
      CBORObject obj = null;
      if (rank == 1) {
        // Most common case: the array is one-dimensional
        obj = CBORObject.NewArray();
        int len = arr.GetLength(0);
        for (int i = 0; i < len; ++i) {
          obj.Add(CBORObject.FromObject(arr.GetValue(i)));
        }
        return obj;
      }
      int[] index = new int[rank];
      obj = CBORObject.NewArray();
      FromArrayRecursive(arr, index, 0, obj);
      return obj;
    }

    public static object EnumToObject(Enum value) {
      Type t = Enum.GetUnderlyingType(value.GetType());
      if (t.Equals(typeof(ulong))) {
        byte[] data = new byte[13];
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
        return BigInteger.fromByteArray(data, true);
      } else if (t.Equals(typeof(long))) {
        return Convert.ToInt64(value);
      } else if (t.Equals(typeof(uint))) {
        return Convert.ToInt64(value);
      } else {
        return Convert.ToInt32(value);
      }
    }

    public static object FindOneArgumentMethod(object obj, string name, Type argtype) {
      return obj.GetType().GetMethod(name, new Type[] { argtype });
    }

    public static object InvokeOneArgumentMethod(object methodInfo, object obj, object argument) {
      return ((MethodInfo)methodInfo).Invoke(obj, new object[] { argument });
    }

    public static IEnumerable<KeyValuePair<string, object>> GetProperties(Object o) {
      foreach (PropertyData key in GetPropertyList(o.GetType())) {
        yield return new KeyValuePair<string, object>(key.Name, key.Prop.GetValue(o, null));
      }
    }
  }
}
