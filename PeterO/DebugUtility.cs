/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO {
  internal static class DebugUtility {
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(string str) {
      Type type = Type.GetType("System.Console");
      var types = new[] { typeof(String) };
      type.GetMethod("WriteLine", types).Invoke(
        type,
        new object[] { str });
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(string format, params object[] args) {
      Log(String.Format(format, args));
    }
  }
}
