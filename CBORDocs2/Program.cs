/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;

namespace CBORDocs {
  internal class Program {
    public static void Main(string[] args) {
      if (args.Length < 2 || String.IsNullOrEmpty(args[args.Length - 2]) ||
        String.IsNullOrEmpty(args[args.Length - 1])) {
        Console.WriteLine("Usage: CBORDocs2 <dllfile> <docpath>");
        return;
      }
      PeterO.DocGen.DocGenerator.Generate(args[args.Length - 2],
        args[args.Length - 1]);
    }
  }
}
