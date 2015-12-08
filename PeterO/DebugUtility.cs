/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
#if DEBUG
using System;
using System.Reflection;

namespace PeterO {
  internal static class DebugUtility {
    private static MethodInfo GetTypeMethod(Type t, string name, Type[]
      parameters) {
      return t.GetRuntimeMethod(name, parameters);
    }

    public static void Log(string str) {
      Type type = Type.GetType("System.Console");
      var types = new[] { typeof(String) };
      GetTypeMethod(type,"WriteLine", types).Invoke(
        type,
        new object[] { str });
    }

    public static void Log(string format, params object[] args) {
      Log(String.Format(format, args));
    }
  }
}
#endif
