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
using NuDoq;

namespace PeterO.DocGen {
  internal class MemberSummaryVisitor : Visitor, IComparer<object> {
    private readonly SortedDictionary<object, StringBuilder> docs;
    private string summaryString;

    public override string ToString() {
      return this.summaryString;
    }

    public MemberSummaryVisitor() {
      this.docs = new SortedDictionary<object, StringBuilder>(this);
      this.summaryString = String.Empty;
    }

    public static string MemberName(object obj) {
      return (obj is Type) ? (((Type)obj).FullName) : ((obj is MethodInfo) ?
        ((MethodInfo)obj).Name : ((obj is PropertyInfo) ?
        ((PropertyInfo)obj).Name : ((obj is FieldInfo) ?
        ((FieldInfo)obj).Name : (obj.ToString())))); } public static string
        MemberAnchor(object obj) {
      string anchor = String.Empty;
      if (obj is Type) {
        anchor = ((Type)obj).FullName;
      } else if (obj is MethodInfo) {
        anchor = (((MethodInfo)obj).Name.IndexOf(
          "op_",
          StringComparison.Ordinal) == 0) ? ((MethodInfo)obj).Name :
            DocVisitor.FormatMethod((MethodInfo)obj, true); } else {
 anchor = (obj is PropertyInfo) ?
   DocVisitor.FormatProperty((PropertyInfo)obj, true) : ((obj is FieldInfo)?
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

    public void Finish() {
      var sb = new StringBuilder();
      string finalString;
      sb.Append("### Member Summary\n");
      foreach (var key in this.docs.Keys) {
        finalString = this.docs[key].ToString();
        var typeName = FormatMember(key);
        typeName = typeName.Replace("&", "&amp;");
        typeName = typeName.Replace("<", "&lt;");
        typeName = typeName.Replace(">", "&gt;");
        typeName = "[" + typeName + "](#" + MemberAnchor(key) + ")";
        finalString = Regex.Replace(finalString, "\\s+", " ");
        if (finalString.IndexOf(".", StringComparison.Ordinal) >= 0) {
          finalString = finalString.Substring(
  0,
  finalString.IndexOf(".", StringComparison.Ordinal) + 1);
        }
        sb.Append("* <code>" + typeName + "</code> - ");
        sb.Append(finalString + "\n");
      }
      finalString = TypeVisitor.NormalizeLines(sb.ToString());
      this.summaryString = finalString;
    }

    public override void VisitMember(Member member) {
      object info = member.Info;
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
        base.VisitMember(member);
        return;
      }
      if (!this.docs.ContainsKey(info)) {
        var docVisitor = new StringBuilder();
        this.docs[info] = docVisitor;
      }
      foreach (var element in member.Elements) {
        if (element is Summary) {
          var text = element.ToText();
          this.docs[info].Append(text);
          this.docs[info].Append("\r\n");
        }
      }
      base.VisitMember(member);
    }

    /// <summary>Compares a Type object with a Type.</summary>
    /// <param name='x'>The parameter <paramref name='x'/> is not
    /// documented yet.</param>
    /// <param name='y'>A Type object.</param>
    /// <returns>Zero if both values are equal; a negative number if
    /// <paramref name='x'/> is less than <paramref name='y'/>, or a
    /// positive number if <paramref name='x'/> is greater than <paramref
    /// name='y'/>.</returns>
    public int Compare(object x, object y) {
      return string.Compare(
        MemberAnchor(x),
        MemberAnchor(y),
        StringComparison.Ordinal);
    }
  }
}
