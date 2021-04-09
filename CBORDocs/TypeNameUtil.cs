using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PeterO.DocGen {
  internal static class TypeNameUtil {
    public static string SimpleTypeName(Type t) {
      if (!t.IsNested) {
        return t.Namespace + "." + TypeNameUtil.UndecorateTypeName(t.Name);
      } else {
        Type nt = t;
        var types = new List<Type>();
        types.Add(t);
        var sb = new StringBuilder().Append(t.Namespace);
        while (nt != null && nt.IsNested) {
          types.Add(nt.DeclaringType);
          nt = nt.DeclaringType;
        }
        for (var i = types.Count - 1; i >= 0; --i) {
          sb.Append('.').Append(UndecorateTypeName(types[i].Name));
        }
        return sb.ToString();
      }
    }
    public static string XmlDocTypeName(Type t) {
      return XmlDocTypeName(t, false);
    }
    public static string XmlDocTypeName(Type t, bool param) {
      return XmlDocTypeName(t, param, false);
    }
    public static string XmlDocTypeName(Type t, bool param, bool
        genericMethod) {
      var sb = new StringBuilder();
      if (t.IsArray) {
        sb.Append(XmlDocTypeName(t.GetElementType(), param, genericMethod))
          .Append("[]");
        } else if (t.IsPointer) {
        sb.Append(XmlDocTypeName(t.GetElementType(), param, genericMethod))
          .Append('*');
        } else if (t.IsByRef) {
        sb.Append(XmlDocTypeName(t.GetElementType(), param, genericMethod))
          .Append('@');
        } else if (t.IsGenericParameter) {
        var ggastr = Convert.ToString(
          t.GenericParameterPosition,
          System.Globalization.CultureInfo.InvariantCulture);
        sb.Append(genericMethod ? "``" : "`").Append(ggastr);
      } else {
        sb.Append(t.Namespace);
        Type nt = t;
        var types = new List<Type>();
        types.Add(t);
        while (nt != null && nt.IsNested) {
          types.Add(nt.DeclaringType);
          nt = nt.DeclaringType;
        }
        for (var i = types.Count - 1; i >= 0; --i) {
          sb.Append('.').Append(UndecorateTypeName(types[i].Name));
          if (types[i].GetGenericArguments().Length > 0) {
            if (param) {
              sb.Append('{');
              var first = true;
              foreach (var ga in types[i].GetGenericArguments()) {
                if (!first) {
                  sb.Append(',');
                }
                sb.Append(XmlDocTypeName(ga, false, genericMethod));
                first = false;
              }
              sb.Append('}');
            } else {
              var ggastr = Convert.ToString(
                types[i].GetGenericArguments().Length,
                System.Globalization.CultureInfo.InvariantCulture);
              sb.Append('`').Append(ggastr);
            }
          }
        }
      }
      return sb.ToString();
    }

    public static string XmlDocMemberName(object obj) {
      if (obj is Type) {
        return "T:" + XmlDocTypeName((Type)obj);
      }
      if (obj is ConstructorInfo) {
        var cons = obj as ConstructorInfo;
        var msb = new StringBuilder()
          .Append("M:").Append(XmlDocTypeName(cons.DeclaringType))
          .Append(cons.IsStatic ? ".#cctor" : ".#ctor");
        if (cons.GetParameters().Length > 0) {
          msb.Append('(');
          var first = true;
          foreach (var p in cons.GetParameters()) {
            if (!first) {
              msb.Append(',');
            }
            msb.Append(XmlDocTypeName(p.ParameterType, true));
            first = false;
          }
          msb.Append(')');
        }
        return msb.ToString();
      }
      if (obj is MethodInfo) {
        var mi = obj as MethodInfo;
        var msb = new StringBuilder()
          .Append("M:").Append(XmlDocTypeName(mi.DeclaringType))
          .Append('.').Append(mi.Name);
        var gga = mi.GetGenericArguments().Length;
        bool genericMethod = gga > 0;
        if (genericMethod) {
          msb.Append("``");
          var ggastr = Convert.ToString(
            gga,
            System.Globalization.CultureInfo.InvariantCulture);
          msb.Append(ggastr);
        }
        if (mi.GetParameters().Length > 0) {
          msb.Append('(');
          var first = true;
          foreach (var p in mi.GetParameters()) {
            if (!first) {
              msb.Append(',');
            }
            msb.Append(XmlDocTypeName(p.ParameterType, true, genericMethod));
            first = false;
          }
          msb.Append(')');
        }
        if (mi.Name.Equals("op_Explicit", StringComparison.Ordinal) ||
mi.Name.Equals("op_Implicit", StringComparison.Ordinal)) {
          var rt = mi.ReturnType;
          if (rt != null) {
            msb.Append('~').Append(XmlDocTypeName(rt, true, genericMethod));
          }
        }
        return msb.ToString();
      }
      if (obj is PropertyInfo) {
        var pi = obj as PropertyInfo;
        var msb = new StringBuilder().Append("P:")
          .Append(XmlDocTypeName(pi.DeclaringType)).Append('.')
          .Append(pi.Name);
        if (pi.GetIndexParameters().Length > 0) {
          msb.Append('(');
          var first = true;
          foreach (var p in pi.GetIndexParameters()) {
            if (!first) {
              msb.Append(',');
            }
            msb.Append(XmlDocTypeName(p.ParameterType, true));
            first = false;
          }
          msb.Append(')');
        }
        return msb.ToString();
      }
      if (obj is FieldInfo) {
        string m = "F:" + XmlDocTypeName(((FieldInfo)obj).DeclaringType) +
           "." + ((FieldInfo)obj).Name;
        return m;
      }
      return obj.ToString();
    }

    public static string UndecorateTypeName(string name) {
      var idx = name.IndexOf('`');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      idx = name.IndexOf('[');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      idx = name.IndexOf('*');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      idx = name.IndexOf('@');
      if (idx >= 0) {
        name = name.Substring(0, idx);
      }
      return name;
    }
  }
}
