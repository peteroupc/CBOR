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
    internal static void Main(string[] args) {
      var members = DocReader.Read(typeof(CBORObject).Assembly);
      var oldWriter = Console.Out;
      using (var writer = new StreamWriter("../../../APIDocs.md")) {
        var visitor = new TypeVisitor(writer);
        members.Accept(visitor);
        visitor.Finish();
      }
    }
  }
}
