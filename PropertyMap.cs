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
    private sealed class PropertyData {
      public string name;
      public PropertyInfo prop;
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
            PropertyData pd=new PropertyMap.PropertyData();
            pd.name=pi.Name;
            // Convert 'IsXYZ' to 'XYZ'
            if(pd.name.Length>=3 &&
               pd.name[0]=='I' && pd.name[1]=='s' &&
               pd.name[2]>='A' && pd.name[2]=='Z'){
              pd.name=pd.name.Substring(2);
            }
            // Convert to camel case
            if(pd.name[0]>='A' && pd.name[0]<='Z'){
              StringBuilder sb=new System.Text.StringBuilder();
              sb.Append((char)(pd.name.charAt(0)+0x20));
              sb.Append(pd.name.Substring(1));
              pd.name=pd.toString();
            }
            pd.prop=pi;
            ret.Add(pd);
          }
        }
        propertyLists.Add(t, ret);
        return ret;
      }
    }

    public static IEnumerable<KeyValuePair<string, object>> GetProperties(Object o) {
      foreach (PropertyData key in GetPropertyList(o.GetType())) {
        yield return new KeyValuePair<string, object>(key.name, key.prop.GetValue(o, null));
      }
    }
  }
}
