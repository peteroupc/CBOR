using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Test {
    /// <summary>Utility methods for accessing internal APIs via
    /// reflection.</summary>
  public static class Reflect {
    private static IDictionary<string, Type> typeCache = new
      Dictionary<string, Type>();

    // Check every assembly in the AppDomain; by default,
    // Type.GetType looks only in the current assembly.
    private static Type FindType(string type) {
      lock (typeCache) {
      if (typeCache.ContainsKey(type)) {
        return typeCache[type];
      }
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        Type typeObject = Type.GetType(type + "," + assembly.FullName);
        if (typeObject != null) {
          typeCache[type] = typeObject;
          return typeObject;
        }
      }
      return null;
      }
    }

    public static object Construct(string type, params object[] parameters) {
      try {
        return Activator.CreateInstance(
FindType(type),
          BindingFlags.Instance | BindingFlags.Public |
 BindingFlags.NonPublic | BindingFlags.CreateInstance,
 null,
 parameters,
          null);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
    }

    public static object GetMethod(object obj, string name) {
      return obj.GetType().GetMethod(
name,
BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
    }

    private static object GetMethodExtended(
IReflect type,
string name,
bool staticMethod,
int parameterCount) {
      bool haveMethodName = false;
      BindingFlags flags = (staticMethod ? BindingFlags.Static :
        BindingFlags.Instance) | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.InvokeMethod;
      foreach (var method in type.GetMethods(flags)) {
        if (method.Name.Equals(name)) {
          haveMethodName = true;
          if (method.GetParameters().Length == parameterCount) {
            return method;
          }
        }
      }
      return haveMethodName ? type.GetMethod(name, flags) : null;
    }

    public static object InvokeMethod(
object obj,
object method,
params object[] parameters) {
      try {
        return ((MethodInfo)method).Invoke(obj, parameters);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
    }

    public static object Invoke(
object obj,
string name,
params object[] parameters) {
      return InvokeMethod(
obj,
GetMethodExtended(obj.GetType(), name, false, parameters.Length),
parameters);
    }

    public static object InvokeStatic(
string type,
string name,
params object[] parameters) {
      return InvokeMethod(
null,
GetMethodExtended(FindType(type), name, true, parameters.Length),
parameters);
    }

    public static object GetProperty(object obj, string name) {
      return InvokeMethod(
obj,
 obj.GetType().GetProperty(
name,
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).GetGetMethod());
    }

    public static object GetPropertyStatic(string type, string name) {
      return InvokeMethod(
null,
 FindType(
type).GetProperty(
name,
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty).GetGetMethod());
    }

    public static object GetField(object obj, string name) {
      return obj.GetType().GetField(
name,
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField).GetValue(obj);
    }

    public static object GetFieldStatic(string type, string name) {
      return FindType(
type).GetField(
name,
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField).GetValue(null);
    }
  }
}
