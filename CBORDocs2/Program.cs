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

using PeterO.DocGen;
using PeterO.Cbor;

namespace CBORDocs {
  internal class Program {
    public static void Main(string[] args) {
      DocGenerator.Generate(typeof(CBORObject).Assembly, "../../../docs");
    }
  }
}
