/*
Written by Peter O. in 2014.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PeterO.DocGen {
  internal class MemberSummaryVisitor {
    private readonly SortedDictionary<string, StringBuilder> docs;
    private readonly Dictionary<string, string> memberFormats;
    private string summaryString;

    public override string ToString() {
      return this.summaryString;
    }

    public MemberSummaryVisitor() {
      this.docs = new SortedDictionary<string, StringBuilder>();
      this.memberFormats = new Dictionary<string, string>();
      this.summaryString = String.Empty;
    }

    public static string MemberName(object obj) {
      return (obj is Type) ? (((Type)obj).FullName) : ((obj is MethodInfo) ?
        ((MethodInfo)obj).Name : ((obj is PropertyInfo) ?
        ((PropertyInfo)obj).Name : ((obj is FieldInfo) ?

        ((FieldInfo)obj).Name : (obj.ToString())))); }
    public static string MemberAnchor(object obj) {
      string anchor = String.Empty;
      if (obj is Type) {
        anchor = ((Type)obj).FullName;
      } else if (obj is MethodInfo) {
        anchor = (((MethodInfo)obj).Name.IndexOf(
          "op_",
          StringComparison.Ordinal) == 0) ? ((MethodInfo)obj).Name :
            DocVisitor.FormatMethod((MethodInfo)obj, true);
 } else {
 anchor = (obj is PropertyInfo) ?
   DocVisitor.FormatProperty((PropertyInfo)obj, true) :
             ((obj is FieldInfo) ?
     ((FieldInfo)obj).Name : obj.ToString());
}
      anchor = anchor.Trim();
      anchor = Regex.Replace(anchor, "\\(\\)", String.Empty);
      anchor = Regex.Replace(anchor, "\\W+", "_");
      anchor = Regex.Replace(anchor, "_+$", String.Empty);
      return anchor;
    }

    public static string FormatMember(object obj) {
      if (obj is Type) {
        return ((Type)obj).FullName;
      }
      if (obj is MethodInfo) {
        string m = DocVisitor.FormatMethod((MethodInfo)obj, true);
        m = Regex.Replace(m, "\\s+", " ");
        m = m.Trim();
        return m;
      }
      if (obj is PropertyInfo) {
        string m = DocVisitor.FormatProperty((PropertyInfo)obj, true);
        m = Regex.Replace(m, "\\s+", " ");
        m = m.Trim();
        return m;
      }
      if (obj is FieldInfo) {
        string m = DocVisitor.FormatField((FieldInfo)obj);
        m = Regex.Replace(m, "\\s+", " ");
        m = m.Trim();
        return m;
      }
      return obj.ToString();
    }

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

    public void Finish() {
      var sb = new StringBuilder();
      string finalString;
      sb.Append("### Member Summary\n");
      foreach (var key in this.docs.Keys) {
        finalString = this.docs[key].ToString();
        var typeName = this.memberFormats[key];
        typeName = typeName.Replace("&", "&amp;");
        typeName = typeName.Replace("<", "&lt;");
        typeName = typeName.Replace(">", "&gt;");
        typeName = "[" + typeName + "](#" + key + ")";
        finalString = Regex.Replace(finalString, "\\s+", " ");
        if (finalString.IndexOf(".", StringComparison.Ordinal) >= 0) {
          finalString = finalString.Substring(
  0,
  finalString.IndexOf(".", StringComparison.Ordinal) + 1);
        }
        sb.Append("* <code>" + typeName + "</code> - ");
        sb.Append(finalString + "\n");
      }
      finalString = DocGenUtil.NormalizeLines(sb.ToString());
      this.summaryString = finalString;
    }

    public void HandleMember(object info, XmlDoc xmldoc) {
      var isPublicOrProtected = false;
      var typeInfo = info as Type;
      var methodInfo = info as MethodInfo;
      var propertyInfo = info as PropertyInfo;
      var fieldInfo = info as FieldInfo;
      if (methodInfo != null) {
        isPublicOrProtected = methodInfo.IsPublic || methodInfo.IsFamily;
      }
      if (propertyInfo != null) {
        isPublicOrProtected = (propertyInfo.CanRead &&
                    propertyInfo.GetGetMethod().IsPublic ||
                    propertyInfo.GetGetMethod().IsFamily) ||
                    (propertyInfo.CanWrite &&
                    propertyInfo.GetSetMethod().IsPublic ||
                    propertyInfo.GetSetMethod().IsFamily);
      }
      if (fieldInfo != null) {
        isPublicOrProtected = fieldInfo.IsPublic || fieldInfo.IsFamily;
      }
      if (!isPublicOrProtected) {
        return;
      }
      string memberAnchor = MemberAnchor(info);
      this.memberFormats[memberAnchor] = FormatMember(info);
      if (!this.docs.ContainsKey(memberAnchor)) {
        var docVisitor = new StringBuilder();
        this.docs[memberAnchor] = docVisitor;
      }
      string memberFullName = XmlDocMemberName(info);
      var summary = xmldoc?.GetSummary(memberFullName);
      if (summary == null) {
        Console.WriteLine("no summary for " + memberFullName);
      } else {
        this.docs[memberAnchor].Append(summary)
            .Append("\r\n");
      }
    }
  }
}
