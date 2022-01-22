/*
Written by Peter O.

Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PeterO.DocGen {
  internal class TypeVisitor {
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

    public void HandleTypeAndMembers(Type currentType, XmlDoc xmldoc) {
      this.HandleMember(currentType, xmldoc);
      foreach (var m in currentType.GetFields()) {
        if (m.IsSpecialName) {
          continue;
        }
        this.HandleMember(m, xmldoc);
      }
      foreach (var m in currentType.GetConstructors()) {
        if (!m.DeclaringType.Equals(currentType)) {
          continue;
        }
        this.HandleMember(m, xmldoc);
      }
      foreach (var m in currentType.GetMethods()) {
        if (!m.DeclaringType.Equals(currentType)) {
          var dtfn = m.DeclaringType.FullName;
          if (dtfn.IndexOf("System.", StringComparison.Ordinal) != 0) {
            // Console.WriteLine("not declared: " + m);
          }
          continue;
        }
        if (m.IsSpecialName && (
          m.Name.IndexOf("get_", StringComparison.Ordinal) == 0 ||
          m.Name.IndexOf("set_", StringComparison.Ordinal) == 0)) {
          continue;
        }
        this.HandleMember(m, xmldoc);
      }
      foreach (var m in currentType.GetProperties()) {
        if (!m.DeclaringType.Equals(currentType)) {
          var dtfn = m.DeclaringType.FullName;
          if (dtfn.IndexOf("System.", StringComparison.Ordinal) != 0) {
            Console.WriteLine("not declared: " + m);
          }
          continue;
        }
        this.HandleMember(m, xmldoc);
      }
    }

    public void HandleMember(MemberInfo info, XmlDoc xmldoc) {
      Type currentType;
      if (info is Type) {
        currentType = (Type)info;
      } else {
        if (info == null) {
          return;
        }
        currentType = info.ReflectedType;
      }
      if (currentType == null ||
          !(currentType.IsNested ? currentType.IsNestedPublic :
currentType.IsPublic)) {
        return;
      }
      var typeFullName = currentType.FullName;
      if (!this.docs.ContainsKey(typeFullName)) {
        var docVisitor = new DocVisitor();
        this.docs[typeFullName] = docVisitor;
        this.typeIDs[typeFullName] = DocVisitor.GetTypeID(currentType);
        this.memSummaries[typeFullName] = new MemberSummaryVisitor();
      }
      this.docs[typeFullName].HandleMember(info, xmldoc);
      this.memSummaries[typeFullName].HandleMember(info, xmldoc);
    }
  }
}
