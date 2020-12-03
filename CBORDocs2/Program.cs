/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace CBORDocs {
  internal class Program {
    public static void Main(string[] args) {
      if (args.Length < 2 || String.IsNullOrEmpty(args[0]) ||
        String.IsNullOrEmpty(args[1])) {
        Console.WriteLine("Usage: CBORDocs2 <dllfile> <docpath>");
        return;
      }
      PeterO.DocGen.DocGenerator.Generate(args[0], args[1]);
    }
  }
}
