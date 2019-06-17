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
using System.Text.RegularExpressions;
using NuDoq;

namespace PeterO.DocGen {
  internal class TypeVisitor : Visitor {
    private readonly SortedDictionary<string, DocVisitor> docs;
    private readonly Dictionary<string, MemberSummaryVisitor> memSummaries;
    private readonly Dictionary<string, string> typeIDs;
    private readonly string directory;

    public XmlDoc XmlDoc { get; set; }

    public TypeVisitor(string directory) {
      this.XmlDoc = null;
      this.docs = new SortedDictionary<string, DocVisitor>();
      this.memSummaries = new Dictionary<string, MemberSummaryVisitor>();
      this.typeIDs = new Dictionary<string, string>();
      this.directory = directory;
    }

    public void Finish() {
      foreach (var key in this.docs.Keys) {
        var finalString = this.docs[key].ToString();
        this.memSummaries[key].Finish();
        var memSummaryString = this.memSummaries[key].ToString();
        var filename = Path.Combine(
  this.directory,
  this.typeIDs[key] + ".md");
        finalString = DocGenUtil.NormalizeLines(finalString);
        finalString = Regex.Replace(
            finalString,
            "<<<MEMBER_SUMMARY>>>",
            memSummaryString);
        DocGenUtil.FileEdit(filename, finalString);
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
        foreach (var m in currentType.GetFields()) {
          if (m.IsSpecialName) continue;
          this.memSummaries[typeFullName].HandleMember(m, this.XmlDoc);
        }
        foreach (var m in currentType.GetConstructors()) {
          this.memSummaries[typeFullName].HandleMember(m, this.XmlDoc);
        }
        foreach (var m in currentType.GetMethods()) {
          if (!m.DeclaringType.Equals(currentType)) {
            var dtfn = m.DeclaringType.FullName;
            if (dtfn.IndexOf("System.", StringComparison.Ordinal) != 0) {
              Console.WriteLine("not declared: " + m);
            }
            continue;
          }
          if (m.IsSpecialName && (
            m.Name.IndexOf("get_", StringComparison.Ordinal) == 0 ||
            m.Name.IndexOf("set_", StringComparison.Ordinal) == 0)) {
            continue;
          }
          this.memSummaries[typeFullName].HandleMember(m, this.XmlDoc);
        }
        foreach (var m in currentType.GetProperties()) {
          if (!m.DeclaringType.Equals(currentType)) {
            var dtfn = m.DeclaringType.FullName;
            if (dtfn.IndexOf("System.", StringComparison.Ordinal) != 0) {
              Console.WriteLine("not declared: " + m);
            }
            continue;
          }
          this.memSummaries[typeFullName].HandleMember(m, this.XmlDoc);
        }
        foreach (var m in currentType.GetNestedTypes()) {
          this.memSummaries[typeFullName].HandleMember(m, this.XmlDoc);
        }
      }
      this.docs[typeFullName].VisitMember(member);
      if(this.XmlDoc==null){
        this.memSummaries[typeFullName].VisitMember(member);
      }
      base.VisitMember(member);
    }
  }
}
