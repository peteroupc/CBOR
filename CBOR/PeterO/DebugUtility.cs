/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
// Use directives rather than the Conditional attribute,
  // to avoid the chance of logging statements leaking in release builds
#if DEBUGLOG
namespace PeterO {
  internal static class DebugUtility {
    private static readonly object WriterLock = new object();
    private static Action<string> writer;

    [System.Diagnostics.Conditional("DEBUG")]
    public static void SetWriter(Action<string> wr) {
      lock (WriterLock) {
        writer = wr;
      }
    }

    // [RequiresUnreferencedCode("Do not use in AOT or reflection-free
    // contexts.")]
    private static MethodInfo GetTypeMethod(
      Type t,
      string name,
      Type[] parameters) {
#if NET40 || NET20
        return t.GetMethod(name, new[] { parameter });
#else
{
        return t?.GetRuntimeMethod(name, parameters);
      }
#endif
    }

    // [RequiresUnreferencedCode("Do not use in AOT or reflection-free
    // contexts.")]
    public static void Log(string str) {
      var type = Type.GetType("System.Console");
      if (type == null) {
        Action<string> wr = null;
        lock (WriterLock) {
          wr = writer;
        }
        if (wr != null) {
#if !NET20 && !NET40
          System.Diagnostics.Debug.WriteLine(str);
#endif
          wr(str);
          return;
        } else {
#if !NET20 && !NET40
          System.Diagnostics.Debug.WriteLine(str);
          return;
#else
{
 throw new NotSupportedException("System.Console not found");
}
#endif
        }
      }
      Type[] types = new[] { typeof(string) };
      MethodInfo typeMethod = GetTypeMethod(type, "WriteLine", types);
      if (typeMethod != null) {
        typeMethod.Invoke(
          type,
          new object[] { str });
      } else {
        throw new NotSupportedException("System.Console.WriteLine not found");
      }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    // [RequiresUnreferencedCode("Do not use in AOT or reflection-free
    // contexts.")]
    public static void Log(string format, params object[] args) {
      Log(String.Format(
        System.Globalization.CultureInfo.CurrentCulture,
        format,
        args));
    }
  }
}
#endif
