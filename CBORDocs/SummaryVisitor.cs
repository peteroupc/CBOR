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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NuDoq;

namespace PeterO.DocGen {
  internal class SummaryVisitor {
    private sealed class TypeLinkAndBuilder {
      public TypeLinkAndBuilder(Type type) {
        var typeName = DocVisitor.FormatType(type);
        typeName = typeName.Replace("&", "&amp;");
        typeName = typeName.Replace("<", "&lt;");
        typeName = typeName.Replace(">", "&gt;");
        typeName = "[" + typeName + "](" +
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
      sb.Append("## API Documentation\r\n\r\n");
      foreach (var key in this.docs.Keys) {
        finalString = this.docs[key].Builder.ToString();
        if (finalString.IndexOf(".", StringComparison.Ordinal) >= 0) {
          finalString = finalString.Substring(
  0,
  finalString.IndexOf(".", StringComparison.Ordinal) + 1);
        }
        sb.Append(" * " + this.docs[key].TypeLink + " - ");
        sb.Append(finalString + "\n");
      }
      finalString = TypeVisitor.NormalizeLines(
              sb.ToString());
      TypeVisitor.FileEdit(this.filename, finalString);
    }

    private void HandleType(Type currentType, Member member) {
      if (!currentType.IsPublic) return;
      var typeFullName = currentType.FullName;
      if (!this.docs.ContainsKey(typeFullName)) {
        var docVisitor = new TypeLinkAndBuilder(currentType);
        this.docs[typeFullName] = docVisitor;
      }
      foreach (var element in member.Elements) {
        if (element is Summary) {
          var text = element.ToText();
          this.docs[typeFullName].Builder.Append(text)
              .Append("\r\n");
        }
      }
    }

    public void HandleType(Type currentType, XmlDoc xdoc) {
      if (!currentType.IsPublic) return;
      var typeFullName = currentType.FullName;
      if (!this.docs.ContainsKey(typeFullName)) {
        var docVisitor = new TypeLinkAndBuilder(currentType);
        this.docs[typeFullName] = docVisitor;
      }
      var summary = xdoc.GetSummary("T:" + typeFullName);
      if (summary == null) {
        Console.WriteLine("no summary for " + typeFullName);
      } else {
        this.docs[typeFullName].Builder.Append(summary)
            .Append("\r\n");
      }
    }
  }
}
