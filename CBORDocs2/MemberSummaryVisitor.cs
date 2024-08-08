/*
Written by Peter O.

Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
      return (obj is Type) ? ((Type)obj).FullName : ((obj is MethodInfo) ?
((MethodInfo)obj).Name : ((obj is PropertyInfo) ? ((PropertyInfo)obj).Name :
((obj is FieldInfo) ?

        ((FieldInfo)obj).Name : obj.ToString())));
    }

    public static bool IsNonconversionOperator(string name) {
      return name.IndexOf("op_", StringComparison.Ordinal) == 0 &&
        !name.Equals("op_Explicit", StringComparison.Ordinal) &&
!name.Equals("op_Implicit", StringComparison.Ordinal);
    }

    public static string MemberAnchor(object obj) {
      string anchor;
      if (obj is Type) {
        anchor = ((Type)obj).FullName;
      } else if (obj is MethodInfo) {
        string objname = ((MethodInfo)obj).Name;
        anchor = IsNonconversionOperator(objname) ?
            objname : DocVisitor.FormatMethod((MethodInfo)obj, true);
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
        string m = ((Type)obj).FullName;
        m = Regex.Replace(m, "\\+", ".");
        m = Regex.Replace(m, "\\s+", " ");
        m = m.Trim();
        return m;
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

    public void Finish() {
      var sb = new StringBuilder();
      string finalString;
      _ = sb.Append("### Member Summary\n");
      foreach (string key in this.docs.Keys) {
        finalString = this.docs[key].ToString();
        string typeName = this.memberFormats[key];
        typeName = "[" + DocGenUtil.HtmlEscape(typeName) + "](#" + key + ")";
        finalString = Regex.Replace(finalString, "\\s+", " ");
        _ = sb.Append("* <code>" + typeName + "</code> - ");
        _ = sb.Append(finalString + "\n");
      }
      finalString = DocGenUtil.NormalizeLines(sb.ToString());
      this.summaryString = finalString;
    }

    public void HandleMember(object info, XmlDoc xmldoc) {
      var isPublicOrProtected = false;
      var methodInfo = info as MethodInfo;
      var propertyInfo = info as PropertyInfo;
      var fieldInfo = info as FieldInfo;
      if (methodInfo != null) {
        isPublicOrProtected = methodInfo.IsPublic || methodInfo.IsFamily;
      }
      if (propertyInfo != null) {
        isPublicOrProtected = (propertyInfo.CanRead &&
                    (propertyInfo.GetGetMethod().IsPublic ||
                     propertyInfo.GetGetMethod().IsFamily)) ||
                    (propertyInfo.CanWrite &&
                    (propertyInfo.GetSetMethod().IsPublic ||
                     propertyInfo.GetSetMethod().IsFamily));
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
      string memberFullName = TypeNameUtil.XmlDocMemberName(info);
      string summary = SummaryVisitor.GetSummary(
        info as MemberInfo,
        xmldoc,
        memberFullName);
      if (summary == null) {
        Console.WriteLine("no summary for " + memberFullName);
      } else {
        _ = this.docs[memberAnchor].Append(summary)
            .Append("\r\n");
      }
    }
  }
}
