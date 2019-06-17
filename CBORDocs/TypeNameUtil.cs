using System;
using System.Reflection;
using System.Text;

namespace PeterO.DocGen {
  internal static class TypeNameUtil {
    public static string XmlDocMemberName(object obj) {
      if (obj is Type) {
        return "T:" + ((Type)obj).FullName;
      }
      if (obj is MethodInfo) {
        var mi = obj as MethodInfo;
        var msb = new StringBuilder()
          .Append("M:").Append(mi.DeclaringType.FullName)
          .Append(".").Append(mi.Name);
        var gga = mi.GetGenericArguments().Length;
        if (gga > 0) {
          msb.Append("``");
          var ggastr = Convert.ToString(
            gga,
            System.Globalization.CultureInfo.InvariantCulture);
          msb.Append(ggastr);
        }
        if (mi.GetParameters().Length > 0) {
          msb.Append("(");
          var first = true;
          foreach (var p in mi.GetParameters()) {
            if (!first) {
              msb.Append(",");
            }
            msb.Append(p.ParameterType.FullName);
            first = false;
          }
          msb.Append(")");
        }
        return msb.ToString();
      }
      if (obj is PropertyInfo) {
        var pi = obj as PropertyInfo;
        var msb = new StringBuilder().Append("P:")
          .Append(pi.DeclaringType.FullName).Append(".")
          .Append(pi.Name);
        if (pi.GetIndexParameters().Length > 0) {
          msb.Append("(");
          var first = true;
          foreach (var p in pi.GetIndexParameters()) {
            if (!first) {
              msb.Append(",");
            }
            msb.Append(p.ParameterType.FullName);
            first = false;
          }
          msb.Append(")");
        }
        return msb.ToString();
      }
      if (obj is FieldInfo) {
        string m = "F:" + ((FieldInfo)obj).DeclaringType.FullName +
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
      return name;
    }
  }
}
