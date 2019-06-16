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
  internal class TypeVisitor : Visitor {
    private readonly SortedDictionary<string, DocVisitor> docs;
    private readonly Dictionary<string, MemberSummaryVisitor> memSummaries;
    private readonly Dictionary<string, string> typeIDs;
    private readonly string directory;

    public TypeVisitor(string directory) {
      this.docs = new SortedDictionary<string, DocVisitor>();
      this.memSummaries = new Dictionary<string, MemberSummaryVisitor>();
      this.typeIDs = new Dictionary<string, string>();
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
  this.typeIDs[key] + ".md");
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
      var typeFullName = currentType.FullName;
      if (!this.docs.ContainsKey(typeFullName)) {
        var docVisitor = new DocVisitor();
        this.docs[typeFullName] = docVisitor;
        this.typeIDs[typeFullName] = DocVisitor.GetTypeID(currentType);
        this.memSummaries[typeFullName] = new MemberSummaryVisitor();
      }
      this.docs[typeFullName].VisitMember(member);
      this.memSummaries[typeFullName].VisitMember(member);
      base.VisitMember(member);
    }

  }
}
