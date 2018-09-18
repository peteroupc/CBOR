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
  internal class TypeVisitor : Visitor, IComparer<Type> {
    private readonly SortedDictionary<Type, DocVisitor> docs;
    private readonly Dictionary<Type, MemberSummaryVisitor> memSummaries;
    private readonly string directory;

    public TypeVisitor(string directory) {
      this.docs = new SortedDictionary<Type, DocVisitor>(this);
      this.memSummaries = new Dictionary<Type, MemberSummaryVisitor>();
      this.directory = directory;
    }

    public static string NormalizeLines(string x) {
      if (String.IsNullOrEmpty(x)) {
 return x;
}
      x = Regex.Replace(x, @"[ \t]+(?=[\r\n]|$)", String.Empty);
      x = Regex.Replace(x, @"\r?\n(\r?\n)+", "\n\n");
      x = Regex.Replace(x, @"\r?\n", "\n");
      x = Regex.Replace(x, @"^\s*", String.Empty);
      x = Regex.Replace(x, @"\s+$", String.Empty);
      return x + "\n";
    }

    public static void FileEdit(string filename, string newString) {
      string oldString = null;
      try {
           oldString = File.ReadAllText(filename);
      } catch (IOException) {
           oldString = null;
      }
      if (oldString == null || !oldString.Equals(newString)) {
             File.WriteAllText(filename, newString);
      }
    }

    public void Finish() {
      foreach (var key in this.docs.Keys) {
        var finalString = this.docs[key].ToString();
        this.memSummaries[key].Finish();
        var memSummaryString = this.memSummaries[key].ToString();
        var filename = Path.Combine(
  this.directory,
  DocVisitor.GetTypeID(key) + ".md");
        finalString = NormalizeLines(finalString);
        finalString = Regex.Replace(
            finalString,
            "<<<MEMBER_SUMMARY>>>",
            memSummaryString);
        FileEdit(filename, finalString);
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
        var docVisitor = new DocVisitor();
        this.docs[currentType] = docVisitor;
        this.memSummaries[currentType] = new MemberSummaryVisitor();
      }
      this.docs[currentType].VisitMember(member);
      this.memSummaries[currentType].VisitMember(member);
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
    public int Compare(Type x, Type y) {
      return string.Compare(x.FullName, y.FullName, StringComparison.Ordinal);
    }
  }
}
