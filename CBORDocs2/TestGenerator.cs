using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PeterO.DocGen {
  public static class TestGenerator {
    public static void GenerateTests(Type type, string directory) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      string name = TypeNameUtil.UndecorateTypeName(type.Name);
      var builder = new StringBuilder();
      _ = Directory.CreateDirectory(directory);
      _ = builder.Append("using System;\n");
      _ = builder.Append("using System.Collections.Generic;\n");
      _ = builder.Append("using System.Text;\n");
      _ = builder.Append("using " + type.Namespace + ";\n");
      _ = builder.Append("using" +
"\u0020Microsoft.VisualStudio.TestTools.UnitTesting;\n");
      _ = builder.Append("namespace Test {\n");
      _ = builder.Append(" [TestClass]\n");
      _ = builder.Append(" public partial class " + name + "Test {\n");
      var methods = new SortedSet<string>();
      var hasPublicConstructor = false;
      foreach (ConstructorInfo method in type.GetConstructors()) {
        if (!method.IsPublic) {
          continue;
        }
        hasPublicConstructor = true;
        break;
      }
      if (hasPublicConstructor) {
        _ = methods.Add("Constructor");
      }
      foreach (MethodInfo method in type.GetMethods()) {
        if (!method.IsPublic) {
          continue;
        }
        if (!method.DeclaringType.Equals(method.ReflectedType)) {
          continue;
        }
        string methodName = method.Name;
        if (methodName.StartsWith("get_", StringComparison.Ordinal)) {
          methodName = methodName[4..];
        } else if (methodName.StartsWith("set_", StringComparison.Ordinal)) {
          methodName = methodName[4..];
        } else if (methodName.StartsWith("op_", StringComparison.Ordinal)) {
          methodName = "Operator" + methodName[3..];
        }
        if (methodName.StartsWith(".ctor", StringComparison.Ordinal)) {
          methodName = "Constructor";
        }
        if (methodName.StartsWith(".cctor", StringComparison.Ordinal)) {
          methodName = "StaticConstructor";
        }
        if (methodName.Length == 0) {
          continue;
        }
        methodName = PeterO.DataUtilities.ToUpperCaseAscii(
          methodName[..1]) + methodName[1..];
        _ = methods.Add(methodName);
      }
      if (methods.Count == 0) {
        // no tests to write
        return;
      }
      if (methods.Contains("Constructor")) {
        _ = builder.Append(" [TestMethod]\n");
        _ = builder.Append(" public void TestConstructor() {\n");
        _ = builder.Append(" // not implemented yet\n");
        _ = builder.Append(" }\n");
      }
      foreach (string methodName in methods) {
        if (methodName.Equals("Constructor", StringComparison.Ordinal)) {
          continue;
        }
        _ = builder.Append(" [TestMethod]\n");
        _ = builder.Append(" public void Test" + methodName + "() {\n");
        _ = builder.Append(" // not implemented yet\n");
        _ = builder.Append(" }\n");
      }
      _ = builder.Append(" }\n");
      _ = builder.Append('}');
      string filename = Path.Combine(directory, name + "Test.cs");
      if (!File.Exists(filename)) {
        File.WriteAllText(filename, builder.ToString());
      }
    }

    public static void GenerateTests(Assembly assembly, string directory) {
      if (assembly == null) {
        throw new ArgumentNullException(nameof(assembly));
      }
      foreach (Type type in assembly.GetTypes()) {
        if (!type.IsPublic || type.IsInterface) {
          continue;
        }
        GenerateTests(type, directory);
      }
    }
  }
}
