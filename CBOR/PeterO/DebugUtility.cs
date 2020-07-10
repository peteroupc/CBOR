/*
Written by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Reflection;

namespace PeterO {
  internal static class DebugUtility {
    private static Action<string> writer = null;
    internal static readonly object writerLock = new Object();

    [System.Diagnostics.Conditional("DEBUG")]
    public static void SetWriter(Action<string> wr) {
       lock (writerLock) {
         writer = wr;
       }
    }

    private static MethodInfo GetTypeMethod(
      Type t,
      string name,
      Type[] parameters) {
      #if NET40 || NET20
        return t.GetMethod(name, parameters);
      #else
{
 return t?.GetRuntimeMethod(name, parameters);
}
      #endif
    }

    public static void Log(string str) {
      Type type = Type.GetType("System.Console");
      if (type == null) {
         Action<string> wr = null;
         lock (writerLock) {
           wr = writer;
         }
         if (wr != null) {
          #if !NET20
          System.Diagnostics.Debug.WriteLine(str);
          #endif
          wr(str);
          return;
         } else {
            throw new NotSupportedException("System.Console not found");
         }
      }
      var types = new[] { typeof(string) };
      var typeMethod = GetTypeMethod(type, "WriteLine", types);
      if (typeMethod != null) {
        typeMethod.Invoke(
          type,
          new object[] { str });
      } else {
        throw new NotSupportedException("System.Console.WriteLine not found");
      }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(string format, params object[] args) {
      Log(String.Format(
        System.Globalization.CultureInfo.CurrentCulture,
        format,
        args));
    }
  }
}
