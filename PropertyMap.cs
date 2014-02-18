/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PeterO
{
    /// <summary>Description of PropertyMap.</summary>
  internal class PropertyMap
  {
    private static IDictionary<Type, IList<PropertyInfo>> propertyLists =
      new Dictionary<Type, IList<PropertyInfo>>();

    private static IList<PropertyInfo> GetPropertyList(Type t) {
       lock (propertyLists) {
        IList<PropertyInfo> ret;
        if (propertyLists.TryGetValue(t, out ret)) {
          return ret;
        }
        ret = new List<PropertyInfo>();
        bool anonymous = t.Name.Contains("__AnonymousType");
         foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
          if (pi.CanRead && (pi.CanWrite || anonymous) && pi.GetIndexParameters().Length == 0) {
            ret.Add(pi);
          }
        }
        propertyLists.Add(t, ret);
        return ret;
      }
    }

    public static IEnumerable<KeyValuePair<string, object>> GetProperties(Object o) {
       foreach (PropertyInfo key in GetPropertyList(o.GetType())) {
        yield return new KeyValuePair<string, object>(key.Name, key.GetValue(o, null));
      }
    }
  }
}
