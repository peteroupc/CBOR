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
  internal class TypeVisitor : Visitor, IComparer<Type> {
    private SortedDictionary<Type, DocVisitor> docs;
    internal string directory;

    public TypeVisitor(string directory) {
      this.docs = new SortedDictionary<Type, DocVisitor>(this);
      this.directory = directory;
    }

    public void Finish() {
      foreach (var key in this.docs.Keys) {
        string finalString = this.docs[key].ToString();
        string filename = Path.Combine(this.directory, DocVisitor.GetTypeID(key) + ".md");
        using (var writer = new StreamWriter(filename, false, Encoding.UTF8)) {
          finalString = Regex.Replace(finalString, @"\r?\n(\r?\n)+", "\r\n\r\n");
          writer.WriteLine(finalString);
        }
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
      }
      this.docs[currentType].VisitMember(member);
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
