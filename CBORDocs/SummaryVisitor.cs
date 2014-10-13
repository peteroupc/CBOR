/*
Written in 2014 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ClariusLabs.NuDoc;

namespace PeterO.DocGen {
  internal class SummaryVisitor : Visitor, IComparer<Type> {
    private SortedDictionary<Type, StringBuilder> docs;
    private TextWriter writer;

    public SummaryVisitor(TextWriter writer) {
      this.docs = new SortedDictionary<Type, StringBuilder>(this);
      this.writer = writer;
    }

    public void Finish() {
      foreach (var key in this.docs.Keys) {
        string finalString = this.docs[key].ToString();
        string typeName = DocVisitor.FormatType(key);
        typeName = typeName.Replace("&", "&amp;");
        typeName = typeName.Replace("<", "&lt;");
        typeName = typeName.Replace(">", "&gt;");
        typeName = "[" + typeName + "](" + DocVisitor.GetTypeID(key) + ".md)";
        if (finalString.IndexOf(".", StringComparison.Ordinal) >= 0) {
          finalString = finalString.Substring(
0,
finalString.IndexOf(".", StringComparison.Ordinal) + 1);
        }
        finalString = Regex.Replace(finalString, @"\r?\n(\r?\n)+", "\r\n\r\n");
        this.writer.Write(" * " + typeName + " - ");
        this.writer.WriteLine(finalString);
      }
    }

    public override void VisitMember(Member member) {
      Type currentType;
      if (member.Info is Type) {
        currentType = (Type)member.Info;
      } else {
        return;
      }
      if (currentType == null || !currentType.IsPublic) {
        return;
      }
      if (!this.docs.ContainsKey(currentType)) {
        var docVisitor = new StringBuilder();
        this.docs[currentType] = docVisitor;
      }
      foreach (var element in member.Elements) {
        if (element is Summary) {
          string text = element.ToText();
          this.docs[currentType].Append(text);
          this.docs[currentType].Append("\r\n");
        }
      }
      base.VisitMember(member);
    }

    /// <summary>Compares a Type object with a Type.</summary>
    /// <summary>Compares a Type object with a Type.</summary>
    /// <param name='x'>A Type object.</param>
    /// <param name='y'>A Type object. (2).</param>
    /// <returns>Zero if both values are equal; a negative number if <paramref
    /// name='x'/> is less than <paramref name='y'/> , or a positive number if
    /// <paramref name='x'/> is greater than <paramref name='y'/> .</returns>
    public int Compare(Type x, Type y) {
      return string.Compare(x.FullName, y.FullName, StringComparison.Ordinal);
    }
  }
}
