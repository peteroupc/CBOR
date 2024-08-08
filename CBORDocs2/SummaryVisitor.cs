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

namespace PeterO.DocGen {
  internal class SummaryVisitor {
    private sealed class TypeLinkAndBuilder {
      public TypeLinkAndBuilder(Type type) {
        string typeName = DocVisitor.FormatType(type);
        typeName = "[" + DocGenUtil.HtmlEscape(typeName) + "](" +
DocVisitor.GetTypeID(type) + ".md)";
        this.TypeLink = typeName;
        this.Builder = new StringBuilder();
      }

      public string TypeLink { get; }

      public StringBuilder Builder { get; }
    }

    private readonly SortedDictionary<string, TypeLinkAndBuilder> docs;
    private readonly string filename;

    public SummaryVisitor(string filename) {
      this.docs = new SortedDictionary<string, TypeLinkAndBuilder>();
      this.filename = filename;
    }

    public void Finish() {
      var sb = new StringBuilder();
      string finalString;
      _ = sb.Append("## API Documentation\r\n\r\n");
      foreach (string key in this.docs.Keys) {
        finalString = this.docs[key].Builder.ToString();
        _ = sb.Append(" * " + this.docs[key].TypeLink + " - ");
        _ = sb.Append(finalString + "\n");
      }
      finalString = DocGenUtil.NormalizeLines(
              sb.ToString());
      DocGenUtil.FileEdit(this.filename, finalString);
    }

    internal static string GetSummary(MemberInfo info, XmlDoc xdoc, string
memberName) {
      string summary;
      var attr = info?.GetCustomAttribute(typeof(ObsoleteAttribute)) as
                  ObsoleteAttribute;
      summary = (attr != null) ?
         ("<b>Deprecated:</b> " + DocGenUtil.HtmlEscape(attr.Message)) :
         xdoc?.GetSummary(memberName);
      if (summary != null && attr == null &&
        summary.IndexOf(".", StringComparison.Ordinal) >= 0) {
        summary = summary[
..(summary.IndexOf(".", StringComparison.Ordinal) + 1)];
      }
      return summary;
    }

    public void HandleType(Type currentType, XmlDoc xdoc) {
      if (!(currentType.IsNested ? currentType.IsNestedPublic :
currentType.IsPublic)) {
        return;
      }
      string typeFullName = currentType.FullName;
      if (!this.docs.ContainsKey(typeFullName)) {
        var docVisitor = new TypeLinkAndBuilder(currentType);
        this.docs[typeFullName] = docVisitor;
      }
      string summary = GetSummary(
        currentType,
        xdoc,
        TypeNameUtil.XmlDocMemberName(currentType));
      if (summary == null) {
        Console.WriteLine("no summary for " + typeFullName);
      } else {
        _ = this.docs[typeFullName].Builder.Append(summary)
            .Append("\r\n");
      }
    }
  }
}
