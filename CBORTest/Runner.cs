/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Reflection;
using NUnit.Framework;

namespace PeterO {
    /// <summary>Description of Runner.</summary>
  public class Runner {
    public static void Main() {
      new Test.CBORTest().TestRandomNonsense();
      /* MethodInfo[] methods;
      object bi;
      methods = typeof(Test.BigIntegerTest).GetMethods();
      bi = new Test.BigIntegerTest();
      for (var i = 0; i < 10; ++i) {
        foreach (var method in methods) {
          if (method.GetParameters().Length == 0) {
            Console.WriteLine(method.Name);
            method.Invoke(bi, new object[] { });
          }
        }
      }
      */
    }
  }
}
