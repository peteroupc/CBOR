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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ClariusLabs.NuDoc;
using PeterO.Cbor;

namespace CBORDocs {
  internal class Program {
    public static void Main(string[] args) {
      var directory = "../../../docs";
      Directory.CreateDirectory(directory);
      var members = DocReader.Read(typeof(CBORObject).Assembly);
      var oldWriter = Console.Out;
      TypeVisitor visitor = new TypeVisitor(directory);
      members.Accept(visitor);
      visitor.Finish();
      using (var writer = new StreamWriter(Path.Combine(directory,"APIDocs.md"), false, Encoding.UTF8)) {
        SummaryVisitor visitor2 = new SummaryVisitor(writer);
        members.Accept(visitor2);
        visitor2.Finish();
      }
    }
  }
}
