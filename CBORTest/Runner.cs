/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Reflection;
using NUnit.Framework;
using PeterO.Cbor;

namespace PeterO {
  /// <summary>Description of Runner.</summary>
  public static class Runner {
    private static bool HasAttribute(Type mi, Type t) {
      foreach (object a in mi.GetCustomAttributes(t, false)) {
        if (t.IsInstanceOfType(a)) {
          return true;
        }
      }
      return false;
    }

    private static bool HasAttribute(MethodInfo mi, Type t) {
      foreach (object a in mi.GetCustomAttributes(t, false)) {
        if (t.IsInstanceOfType(a)) {
          return true;
        }
      }
      return false;
    }

    public static void Main() {
      const String ValueParam = "TestReadWriteInt";
      // Run all the tests in this assembly
      foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
        if (!HasAttribute(type, typeof(TestFixtureAttribute))) {
          continue;
        }
        object test = Activator.CreateInstance(type);
        var setup = type.GetMethod("SetUp");
        if (setup != null) {
          setup.Invoke(test, new object[] { });
        }
        foreach (var method in test.GetType().GetMethods()) {
          if (!HasAttribute(method, typeof(TestAttribute))) {
            continue;
          }
          if (!String.IsNullOrEmpty(ValueParam)) {
            if (method.Name.IndexOf(ValueParam, StringComparison.Ordinal) <
              0) {
              continue;
            }
          }
          try {
            method.Invoke(test, new object[] { });
          } catch (TargetInvocationException e) {
            Console.WriteLine("::: " + type.FullName + "." + method.Name);
            Console.WriteLine(e.InnerException.GetType().FullName);
            string message = e.InnerException.Message;
            Console.WriteLine(message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.InnerException.StackTrace);
          }
        }
      }
    }
  }
}
