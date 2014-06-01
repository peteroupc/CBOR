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
using System.Threading.Tasks;

using ClariusLabs.NuDoc;

namespace CBORDocs {
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
        finalString = Regex.Replace(finalString, @"\r?\n(\r?\n)+", "\r\n\r\n");
        this.writer.WriteLine(finalString);
      }
    }

    public override void VisitMember(Member member) {
      Type currentType;
      if (member.Info is Type) {
        currentType = (Type)member.Info;
      } else {
        if (member.Info == null) {
          return;
        }
        currentType = member.Info.ReflectedType;
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
          if (text.IndexOf(".") >= 0) {
            text = text.Substring(0, text.IndexOf("."));
          }
          this.docs[currentType].Append("\r\n");
        }
      }
      base.VisitMember(member);
    }

    /// <summary>Compares a Type object with a Type.</summary>
    /// <returns>Zero if both values are equal; a negative number if <paramref
    /// name='x'/> is less than <paramref name='y'/>, or a positive number
    /// if <paramref name='x'/> is greater than <paramref name='y'/>.</returns>
    /// <param name='x'>A Type object.</param>
    /// <param name='y'>A Type object. (2).</param>
    public int Compare(Type x, Type y) {
      return x.FullName.CompareTo(y.FullName);
    }
  }
}
