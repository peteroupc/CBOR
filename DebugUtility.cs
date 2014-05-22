/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Reflection;

namespace PeterO {
    /// <summary>Description of DebugUtility.</summary>
  internal class DebugUtility
  {
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(string str) {
      Type type = Type.GetType("System.Console");
      type.GetMethod("WriteLine", new Type[] { typeof(String) }).Invoke(type, new object[] { str });
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(string format, params object[] args) {
      Log(String.Format(format, args));
    }
  }
}
