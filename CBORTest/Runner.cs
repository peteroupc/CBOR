/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Reflection;
using Test;

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

    public static bool Extra() {
      var rand = new RandomGenerator();
      for (int i = 0; i < 20; ++i) {
        var array = new byte[rand.UniformInt(100000) + 1];
        _ = rand.GetBytes(array, 0, array.Length);
        DateTime utcn = DateTime.UtcNow;
        CBORTest.TestRandomOne(array);
        TimeSpan span = DateTime.UtcNow - utcn;
        if (span.Seconds > 3) {
          Console.WriteLine("----" + i + ": " + span.Seconds + " " +
array.Length);
        }
      }
      return false;
    }
    /* public static void Main() {
          if (!Extra()) {
            return;
          }
          const String ValueParam = "TestBadDateFields";
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
                Console.WriteLine("::: " + type.FullName + "." + method.Name);
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
    */
  }
}
